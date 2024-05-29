window.googleMaps = {
	map: null,
    iniciar: async function (latitud,longitud) {
		const { Map,Marker } = await google.maps.importLibrary("maps");

		this.map = new Map(document.getElementById("map"), {
			center: { lat:  latitud || 40.48205 , lng:  longitud || -3.35996 }, //sino pasan valores, por defecto selec.Alcala
			zoom: 14,
		});

		console.log('valor de this.map...', this.map);

		//---codigo de prueba para ver si sobre el mapa creo marcadores cuando hago click sobre el mismo...
	//	google.maps.event.addListener( this.map, "click",
	//		(event) => {
	//			console.log("has pulsado sobre el mapa...añadiendo marcador...", event.latLng);
	//			new google.maps.Marker(
	//						{
	//							position: event.latLng,
	//							label: 'prueba Marcador....',
	//							map: this.map,
	//						}
	//			);
	//		}
	//	);
	},
	calculoCoords: async function (direccion) {
        try {
			const { Geocoder } = await google.maps.importLibrary("maps");

			console.log('direccion a obtener coordenadas desde js...', direccion);

			var geocoder = new google.maps.Geocoder();
			var _respuesta = await geocoder.geocode({ 'address': direccion });
			console.log('respuesta de GEOCODING para esa direccion...', _respuesta);

			//formato objeto latLng: https://developers.google.com/maps/documentation/javascript/reference/coordinates?hl=es-419#LatLng
			var _resp = _respuesta.results[0].geometry.location; 

			//puedes usar los metodos de la clase LatLng .lat() y .lng() para obtener la latitud y longitud de la direccion
			var _returnValue = { "lat": _resp.lat(), "lng": _resp.lng() }
			console.log('a devolver...', _returnValue);

			return JSON.stringify(_returnValue);

			//o directamente usar el metodo .ToJSON() de esta clase, como resultado devuelve objeto clase: LatLngLiteral
			//q es un objeto json con el formato: { lat: ..., lng: ... }
			//return JSON.stringify(_resp.ToJSON());

		 

        } catch (e) {
			alert('Geocode was not successful for the following reason: ' + e);

        }
	},
	crearMarkers: async function (inmuebles) {
		//funcion para crear un Marker y InfoWindow para cada Inmueble en el mapa...
		console.log('inmuebles a procesar...', inmuebles);
		console.log('valor de this.map...', this.map);

		//objeto InfoWindow que puede ser compartido entre todos los marcadores...su contenido variara en funcion del
		//marcador pulsado...
		const infoWindow = new google.maps.InfoWindow();

		inmuebles.forEach(
			inmueble => {
							const coordsInmueble = new google.maps.LatLng(inmueble.coordsInmueble.latitud, inmueble.coordsInmueble.longitud);
							console.log('objeto LatLng creado para ese inmueble y su MARKER...', coordsInmueble);

							const marker = new google.maps.Marker(
								{
									position: coordsInmueble,
									label: inmueble.precio + ' €',
									map: this.map, //<--- si no le pasas esta prop. puedes especificar el mapa donde añadir el marcador con metodo .setMap(map) del objeto Marker
									optimized: false
								}
							);

							marker.addListener('click', (ev) => {
					//cuando hago click sobre el marcador desplieguo objeto InfoWindow con nuevo contenido del inmueble pulsado..
					//1º debo cerrar el objeto infoWindow si estuviera desplegado con la info de un marcador anterior...
					infoWindow.close();
					infoWindow.setContent(
						`<div class="container" style="width:250px;">
					<div class="row">
						<div class="col">
							<img src=${inmueble.galeriaFotos[0]} style="width: 100%; max-width: 250px;" alt="...imagen inmueble...">
						</div>
					</div>
					<div class="row">
						<div class="col d-flex flex-row justify-content-start">
							<strong>${inmueble.precio} €</strong>
						</div>
					</div>
					<div class="row">
						<div class="col">
							<span class="text-mutted">${inmueble.direcInmueble.munDirec.DMUN50}</span>
						</div>
					</div>
					<div class="row">
						<div class="col d-flex flex-row justify-content-between">
							<div>${inmueble.numeroHabitaciones} dorm.</div>
							<div>${inmueble.superficieTotal} m2</div>
							<div>${inmueble.numeroBaños} baños</div>
						</div>
					</div>
				</div >`
					);
					infoWindow.open(this.map, marker);

				});


			}
		);

	}
}
window.adminFormModals = {
    modalAbierto: null,
    ShowModal: function (id) {
        if (this.modalAbierto !== null) this.HideModal();
        this.modalAbierto = new bootstrap.Modal(document.getElementById(id));
        this.modalAbierto.show();
    },
    HideModal: function (id) {
        if (this.modalAbierto == null) this.modalAbierto = bootstrap.Modal.getInstance(document.getElementById(id));

        this.modalAbierto.hide();
        this.modalAbierto = null;
    }
}
document.addEventListener("DOMContentLoaded", function () {
    const checkboxes = document.querySelectorAll(".toggle-activo-producto");

    checkboxes.forEach(function (checkbox) {
        checkbox.addEventListener("change", function () {
            const id = this.getAttribute("data-id");
            const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');

            if (!tokenElement) {
                alert("No se encontró el token antifalsificación.");
                this.checked = !this.checked;
                return;
            }

            const token = tokenElement.value;

            fetch("/Productos/ToggleActivo", {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded"
                },
                body: `id=${id}&__RequestVerificationToken=${token}`
            })
                .then(response => {
                    if (!response.ok) {
                        alert("Error al cambiar el estado");
                        this.checked = !this.checked;
                    }
                })
                .catch(error => {
                    console.error("Error en la solicitud:", error);
                    this.checked = !this.checked;
                });
        });
    });
});

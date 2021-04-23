
class Principal {
    userLink(URLactual) {
        let url = "";
        let cadena = URLactual.split("/");
        for (var i = 0; i < cadena.length; i++) {
            if (cadena[i] != "Index") {
                url += cadena[i];
            }
        }
        switch (url) {
            case "UsersRegister":
                document.getElementById('files').addEventListener('change', imageUser, false);
                break;
            case "CustomersRegister":
                document.getElementById('files').addEventListener('change', imageCustomer, false);
                break;
            case "CustomersReports":
                document.getElementById("inlineRadio1").checked = true;
                document.getElementById("inlineRadio2").checked = false;
                document.getElementById("inlineRadio1").disabled = false;
                document.getElementById("inlineRadio2").disabled = true;
                break;
        }
    }
    GetInterests() { }
}

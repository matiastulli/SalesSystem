class Customers extends Uploadpicture {

    SetSection(value) {
        switch (value) {
            case 1:
                document.getElementById("inlineRadio1").checked = true;
                document.getElementById("inlineRadio2").checked = false;
                document.getElementById("inlineRadio1").disabled = false;
                document.getElementById("inlineRadio2").disabled = true;
                break;
            case 2:
                document.getElementById("inlineRadio2").checked = true;
                document.getElementById("inlineRadio1").checked = false;
                document.getElementById("inlineRadio2").disabled = false;
                document.getElementById("inlineRadio1").disabled = true;
                break;
        }
    }
}
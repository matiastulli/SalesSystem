//Video 79
var numberDecimales = (number) => {
    return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
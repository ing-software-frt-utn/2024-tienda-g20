function guardarCarrito(carrito){
    localStorage.setItem("carrito", JSON.stringify(carrito));
}

function cargarCarrito(){
    return JSON.parse(localStorage.getItem("carrito"));
}

function limpiarCarrito(){
    localStorage.removeItem("carrito");
}
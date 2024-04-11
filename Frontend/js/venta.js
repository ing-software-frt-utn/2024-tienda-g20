function onInicioVenta(status){
    if (status!=200){
        alert("Por favor inicie sesión como vendedor");
        window.location.replace("login.html");
        return false;
    }
    setTotal(0);
    return true;
}

function iniciarVenta(){
    api_postAction("Venta/iniciar", onInicioVenta, (carro)=>{
        guardarCarrito(carro);
    });
}

function reiniciarVenta(){
    limpiarCarrito();
    const tabla = document.getElementById("tablaProductos");
    tabla.innerHTML="";
    api_postAction("Venta/iniciar", onInicioVenta, (carro)=>{
        guardarCarrito(carro);
    });
}

document.addEventListener("DOMContentLoaded", function() {
    const listaPagos = document.getElementById("tipoDePago");
    api_getJson("Venta/pagos",
    (obj)=>{
        for (let i=0; i<obj.length; i++){
            listaPagos.innerHTML+="<option value="+i+">"+obj[i].replace(/([A-Z])/g, ' $1').trim()+"</option>";
        }
    }, null);
    iniciarVenta();
    const form = document.getElementById("formTarjeta");
    form.style.display="none";
});

function buscarCliente(){
    const dniCliente = document.getElementById("cliente");
    const nombreCliente = document.getElementById("nombreCliente");
    const lblDni = document.getElementById("dniCliente");
    let dni = parseInt(dniCliente.value);
    if (isNaN(dni) || dni === undefined)
        alert("DNI no válido");
    else{
        api_getJson("Clientes/"+dni,
        (obj)=>{
            nombreCliente.value=obj.razonSocial;
            lblDni.value = obj.dni;
        },
        (status)=>{
            switch(status){
                case 404:
                    alert("Cliente no encontrado");
                    break;
            }
        })
    }
}

function actualizarLista(){
    api_postAndGetJson("Venta/carrito", cargarCarrito(),
    (status)=>{
        switch (status){
            case 401:
                alert("No se encuentra autorizado");
                break;
            case 400:
                alert("Error inesperado");
                break;
            case 200:
                return true;
        }
        return false;
    },
    (lista)=>{
        const tabla = document.getElementById("tablaProductos");
        tabla.innerHTML="";
        let i=0;
        let total=0;
        lista.forEach(element => {
            let tr = document.createElement('tr');
            tr.innerHTML += '<th scope="row">'+i+'</th>';
            tr.innerHTML += '<td>'+element.producto.articulo.descripcion+'</td>';
            tr.innerHTML += '<td>'+element.producto.color.descripcion+'</td>';
            tr.innerHTML += '<td>'+element.producto.talle.descripcion+'</td>';
            tr.innerHTML += '<td>'+element.cantidad+'</td>';
            tr.innerHTML += '<td>'+element.producto.articulo.precioVenta+'</td>';
            tr.innerHTML += '<td>'+element.subtotal+'</td>';
            total += element.subtotal;
            tabla.appendChild(tr);
            i+=1;
        });

        setTotal(total);
    });
}

function setTotal(total){
    const formatter = new Intl.NumberFormat('es-AR', {
        style: 'currency',
        currency: 'USD',
        currencyDisplay: 'narrowSymbol',
        maximumFractionDigits: 2,
        minimumFractionDigits: 2
      });

    const elemTotal = document.getElementById("totalProducto");

    elemTotal.value = formatter.format(total);
}

function buscarArticulo(){
    let prodID = parseInt(document.getElementById("idProducto").value);
    if (isNaN(prodID) || prodID === undefined){
        alert("Datos no válidos");
        return;
    }

    api_getJson("Articulos/"+prodID,
    (art)=>{
        const listaTalles = document.getElementById("listaTalles");
        listaTalles.innerHTML="";
        const listaColores = document.getElementById("listaColores");
        listaColores.innerHTML="";
        let i=0;
        let checked=true;
        art.talles.forEach((t)=>{
            listaTalles.innerHTML+=
                `<input type=\"radio\" class=\"btn-check\" name=\"talle\" id=\"btnTalle${t.id}\" autocomplete=\"off\" ${checked ? "checked" : ""}>\n`
                +`<label class=\"btn btn-outline-primary\" for=\"btnTalle${t.id}\">${t.descripcion}</label>`;
            i++;
            checked=false;
        });
        checked=true;
        art.colores.forEach((c)=>{
            listaColores.innerHTML+=
            `<input type=\"radio\" class=\"btn-check\" name=\"color\" id=\"btnColor${c.id}\" autocomplete=\"off\" ${checked ? "checked" : ""}>\n`
            +`<label class=\"btn btn-outline-primary\" for=\"btnColor${c.id}\">${c.descripcion}</label>`;
            i++;
            checked=false;
        });
    },
    (status)=>{
        switch (status){
            case 401:
                alert("No se encuentra autorizado");
                break;
            case 404:
                alert("Articulo no encontrado");
                break;
            case 400:
                alert("Error inesperado");
                break;
            default:
                alert("Error desconocido");
                break;
        }
    });
}

function agregarProducto(){
    let talle = document.querySelector('input[name="talle"]:checked');
    let color = document.querySelector('input[name="color"]:checked');

    if (!talle || !color){
        alert("Error al seleccionar talle o color");
        return;
    }

    let talleID = parseInt(talle.id.replace('btnTalle',''));
    let colorID = parseInt(color.id.replace('btnColor',''));
    
    if (isNaN(talleID) || isNaN(colorID)){
        alert("Talle o color invalido");
        return;
    }

    let artID = parseInt(document.getElementById("idProducto").value);
    let cantidad = parseInt(document.getElementById("cantidadProducto").value)

    if (isNaN(artID) || artID === undefined || isNaN(cantidad) || cantidad === undefined){
        alert("Datos no válidos");
        return;
    }

    let info = {};
    info.carrito = cargarCarrito();
    info.articulo = artID;
    info.talle = talleID;
    info.color = colorID;
    info.cantidad = cantidad;

    api_postAndGetJson('Venta/agregar', info,
    (status)=>{
        switch (status){
            case 200:
                return true;
            case 401:
                alert("No se encuentra autorizado");
                break;
            case 404:
                alert("Producto no encontrado");
                break;
            case 400:
                alert("Error inesperado");
                break;
        }
        return false;
    },
    (obj)=>{
        guardarCarrito(obj);
        actualizarLista();
    })
}

function quitarProducto(){
    let talle = document.querySelector('input[name="talle"]:checked');
    let color = document.querySelector('input[name="color"]:checked');

    if (!talle || !color){
        alert("Error al seleccionar talle o color");
        return;
    }

    let talleID = parseInt(talle.id.replace('btnTalle',''));
    let colorID = parseInt(color.id.replace('btnColor',''));
    
    if (isNaN(talleID) || isNaN(colorID)){
        alert("Talle o color invalido");
        return;
    }

    let artID = parseInt(document.getElementById("idProducto").value);
    let cantidad = parseInt(document.getElementById("cantidadProducto").value)

    if (isNaN(artID) || artID === undefined || isNaN(cantidad) || cantidad === undefined){
        alert("Datos no válidos");
        return;
    }

    let info = {};
    info.carrito = cargarCarrito();
    info.articulo = artID;
    info.talle = talleID;
    info.color = colorID;
    info.cantidad = cantidad;

    api_postAndGetJson('Venta/quitar', info,
    (status)=>{
        switch (status){
            case 200:
                return true;
            case 401:
                alert("No se encuentra autorizado");
                break;
            case 404:
                alert("Producto no encontrado");
                break;
            case 400:
                alert("Error inesperado");
                break;
        }
        return false;
    },
    (obj)=>{
        guardarCarrito(obj);
        actualizarLista();
    })
}

function pagar(){
    const lblCliente = document.getElementById("dniCliente");tipoDePago
    const cmbPago = document.getElementById("tipoDePago");
    const formTjt = document.getElementById("formTarjeta");

    let dto = {};
    dto.carrito = cargarCarrito();
    dto.tipoDePago = parseInt(cmbPago.value);

    if (lblCliente.value)
        dto.dni = lblCliente.value;

    if (formTjt.style.display===""){
        const nroTarjeta = parseInt(document.getElementById("nroTarjeta").value);
        const vencimiento = document.getElementById("fechaVencimiento").value;
        const seguridad = parseInt(document.getElementById("codSeguridad").value);

        dto.tarjeta = {};
        dto.tarjeta.numeroTarjeta=nroTarjeta;
        dto.tarjeta.vencimiento=vencimiento;
        dto.tarjeta.codigoSeguridad=seguridad;
    }
    
    api_postAndGetJson("Venta/pagar", dto,
    (_)=>{
        return true;
    },
    (obj)=>{
        if (obj.texto === undefined){
            alert("Datos invalidos");
        }else{
            if (obj.status_code == 200){
                const comprobante = obj.objeto.comprobante;
                const nro = comprobante.nroComprobante;
                const tipo = comprobante.tipo.descripcion;
                alert(`Pago exitoso. Comprobante N° ${nro} (${tipo})`);
                console.log(obj);
            }else{
                alert(obj.texto);
                console.log(obj);
            }
        }
    });
}

function tipoDePagoChanged(){
    const tipo = document.getElementById("tipoDePago").value;
    const form = document.getElementById("formTarjeta");
    if (tipo!=0){
        form.style.display="";
    }else{
        form.style.display="none";
    }
}
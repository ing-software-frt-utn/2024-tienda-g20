const PUERTO = 5118;
const ID_HEADER = "Auth-ID";
const HASH_HEADER = "Auth-Token";

function ping(){
    fetch('http://localhost:'+PUERTO+'/api/ping')
    .then(response=>response.text())
    .then(x=>console.log(x));
}

function api_postAction(ruta, statusCallback){
    let requestHeaders ={
        'Content-Type': 'application/json'
    };
    requestHeaders[ID_HEADER] = window.localStorage.getItem("tokenId");
    requestHeaders[HASH_HEADER] = window.localStorage.getItem("tokenHash");

    fetch('http://localhost:'+PUERTO+'/api/'+ruta,{
        method:"POST",
        credentials: "include",
        headers: requestHeaders
    })
    .then(response=>statusCallback(response.status));
}

function api_postAction(ruta, statusCallback, objectCallback){
    let requestHeaders ={
        'Content-Type': 'application/json'
    };
    requestHeaders[ID_HEADER] = window.localStorage.getItem("tokenId");
    requestHeaders[HASH_HEADER] = window.localStorage.getItem("tokenHash");

    fetch('http://localhost:'+PUERTO+'/api/'+ruta,{
        method:"POST",
        credentials: "include",
        headers: requestHeaders
    })
    .then(response=>{
        if (statusCallback(response.status))
            return response.json();
    })
    .then((data)=>objectCallback(data));
}

function api_postJson(ruta, dto, statusCallback){
    let requestHeaders ={
        'Content-Type': 'application/json'
    };
    requestHeaders[ID_HEADER] = window.localStorage.getItem("tokenId");
    requestHeaders[HASH_HEADER] = window.localStorage.getItem("tokenHash");

    fetch('http://localhost:'+PUERTO+'/api/'+ruta,{
        method:"POST",
        credentials: "include",
        headers: requestHeaders,
        body: JSON.stringify(dto),
    })
    .then(response=>statusCallback(response.status));
}

function api_postAndGetJson(ruta, dto, statusCallback, objectCallback, log=false){
    let requestHeaders ={
        'Content-Type': 'application/json'
    };
    requestHeaders[ID_HEADER] = window.localStorage.getItem("tokenId");
    requestHeaders[HASH_HEADER] = window.localStorage.getItem("tokenHash");

    fetch('http://localhost:'+PUERTO+'/api/'+ruta,{
        method:"POST",
        credentials: "include",
        headers: requestHeaders,
        body: JSON.stringify(dto),
    })
    .then(response=>{
        let res = statusCallback(response.status)
        if (log) console.log(response);
        if (res === undefined)
            throw new Error("Solicitud fallida");
        else if (res==true){
            response.json().then(data =>{
                data.status_code=response.status;
                objectCallback(data);
            });
        }
    });
}

function api_getJson(ruta, objectCallback, errorStatus){
    let requestHeaders ={
        'Content-Type': 'application/json'
    };
    requestHeaders[ID_HEADER] = window.localStorage.getItem("tokenId");
    requestHeaders[HASH_HEADER] = window.localStorage.getItem("tokenHash");

    fetch('http://localhost:'+PUERTO+'/api/'+ruta, {
        method: 'GET',
        credentials: "include",
        headers: requestHeaders
    })
    .then(response => {
        if (response.status == 200)
            return response.json();
        else {
            errorStatus(response.status);
            throw new Error("Solicitud fallida");
        }
    })
    .then(objectCallback);
}
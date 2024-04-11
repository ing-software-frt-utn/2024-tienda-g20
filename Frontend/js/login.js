document.addEventListener("DOMContentLoaded", function () {
  const showPasswordCheckbox = document.getElementById("showPassword");
  const passwordField = document.getElementById("password");

  showPasswordCheckbox.addEventListener("change", function () {
    if (showPasswordCheckbox.checked) {
      passwordField.type = "text";
    } else {
      passwordField.type = "password";
    }
  });

  document.getElementById("login-form").addEventListener("submit", function (e) {
    e.preventDefault();

    var formData = new FormData(e.target);
    const formProps = Object.fromEntries(formData);
    login(formProps.username, formProps.password);
  });

  ping();
  localStorage.clear();
});

function getCookieValue(name) {
  const regex = new RegExp(`(^| )${name}=([^;]+)`)
  const match = document.cookie.match(regex)
  if (match) {
    return match[2]
  }
}

function login(user, pass) {
  const dto = {};
  dto.Usuario = user;
  dto.Contra = pass;
  fetch('http://localhost:5118/api/login', {
    method: 'POST', // Specify the method
    credentials: 'include',
    headers: {
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': 'http://127.0.0.1/'
      // Any other headers you want to set
    },
    body: JSON.stringify(dto),
  })
    .then(response => {
      switch (response.status) {
        case 401: throw Error("Contraseña incorrecta");
        case 404: throw Error("Usuario no encontrado");
        case 404: throw Error("Datos invalidos");
        case 200:
          return response.json();
        default:
          throw Error("Respuesta desconocida");
      }
    }) // Parse the response as JSON
    .then(data => {
      const empleado = data.empleado;
      const token = data.token;
      window.localStorage.setItem("tokenId", token.id);
      window.localStorage.setItem("tokenHash", token.hash);
      alert("Bienvenido: " + empleado.apellido + ", " + empleado.nombre)
      window.location.replace("RealizarVenta.html");
      return data;
    })
    .catch((error) => {
      alert(error);
      console.error('Error:', error); // Handle any errors
    });
}
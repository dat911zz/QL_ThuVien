// ======================================================
// Animation
const signUpButton = document.getElementById('signUp');
const signInButton = document.getElementById('signIn');
const container = document.getElementById('container');

signUpButton.addEventListener('click', () => {
	container.classList.add("right-panel-active");
});

signInButton.addEventListener('click', () => {
	container.classList.remove("right-panel-active");
});


// ======================================================
// Process data
var r_submit = document.getElementById("r-b");
/*var r_mail = document.getElementById("r-i-mail");*/
var r_captcha = document.getElementById("txtCaptchaCode");
/*var r_mess = document.getElementById("r-i-mess");*/
//var r_err_e = document.getElementById("r-errEmail");
//var r_err_p = document.getElementById("r-errPass");

var l_submit = document.getElementById("l-b");
var l_name = document.getElementById("l-i-name");
var l_pass = document.getElementById("l-i-pass");

r_submit.onclick = (e) => {
	e.preventDefault()
    if (r_captcha.value == '') {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Vui lòng Captcha!',
            showConfirmButton: true,
            timer: 2500
        })
    }
    else {
        var _url = '/Auth/SignInForUser';
        $.post(_url, { name: r_captcha.value }, function (errData) {
            if (errData != '') {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: errData,
                    showConfirmButton: true,
                    timer: 2500
                }).then(() => {
                    window.location.href = "/";
                })
            }
            else {
                Swal.fire({
                    icon: 'success',
                    title: 'Đã xác minh!',
                    text: errData,
                    showConfirmButton: true,
                    timer: 4000
                }).then((result) => {
                    //Redirect after show modal success
                    window.location.href = '/UserPage/V1/BookLookUp';
                });
            }
        });
    }
}
l_submit.onclick = (e) => {
    e.preventDefault()
    if (l_name.value == '' || l_pass.value == '') {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Vui lòng nhập thông tin!',
            showConfirmButton: true,
            timer: 2500
        })
    }
    else {
        var _url = '/Auth/SignIn'
        $.post(_url, { name: l_name.value, pass: l_pass.value }, function (errData) {
            if (errData != '') {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: errData,
                    showConfirmButton: true,
                    timer: 2500
                })
            }
            else {
                Swal.fire({
                    icon: 'success',
                    title: 'Đăng nhập thành công!',
                    showConfirmButton: true,
                    timer: 2000
                }).then((result) => {
                    //Redirect after show modal success
                    window.location.href = '/';
                });
            }
        });
    }    
}
function checkEmail(email) {
	if (!(/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/.test(email.value))) {
		return false;
	}
	return true;
}
function checkPass(pass) {
	if (pass.value.length <= 1) {
		return false
	}
	return true;
}

particlesJS("particles-js", {
    "particles": {
        "number": {
            "value": 80,
            "density": {
                "enable": true,
                "value_area": 800
            }
        },
        "color": {
            "value": "#ffffff"
        },
        "shape": {
            "type": "circle",
            "stroke": {
                "width": 0,
                "color": "#000000"
            },
            "polygon": {
                "nb_sides": 5
            },
            "image": {
                "src": "img/github.svg",
                "width": 100,
                "height": 100
            }
        },
        "opacity": {
            "value": 0.5,
            "random": true,
            "anim": {
                "enable": false,
                "speed": 1,
                "opacity_min": 0.1,
                "sync": false
            }
        },
        "size": {
            "value": 3,
            "random": true,
            "anim": {
                "enable": false,
                "speed": 40,
                "size_min": 0.1,
                "sync": false
            }
        },
        "line_linked": {
            "enable": true,
            "distance": 208.3116342047702,
            "color": "#ffffff",
            "opacity": 0.4,
            "width": 1
        },
        "move": {
            "enable": true,
            "speed": 6.5,
            "direction": "none",
            "random": true,
            "straight": false,
            "out_mode": "out",
            "bounce": false,
            "attract": {
                "enable": false,
                "rotateX": 600,
                "rotateY": 1200
            }
        }
    },
    "interactivity": {
        "detect_on": "canvas",
        "events": {
            "onhover": {
                "enable": true,
                "mode": "grab"
            },
            "onclick": {
                "enable": true,
                "mode": "repulse"
            },
            "resize": true
        },
        "modes": {
            "grab": {
                "distance": 400,
                "line_linked": {
                    "opacity": 1
                }
            },
            "bubble": {
                "distance": 400,
                "size": 40,
                "duration": 2,
                "opacity": 8,
                "speed": 3
            },
            "repulse": {
                "distance": 200,
                "duration": 0.4
            },
            "push": {
                "particles_nb": 4
            },
            "remove": {
                "particles_nb": 2
            }
        }
    },
    "retina_detect": true
});
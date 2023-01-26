var server = "127.0.0.1";
var tello = false;
var camera = false;
var ws1, ws2;
var line = 0;
var bat = 0;
var ani = -1;
var speed = 10;
var dist = 20;
function elm(id) {
	return document.getElementById(id);
}
function online(sw) {
	elm("connect1").style.display = (sw>0)? "none" : "inline-block";
	elm("takeoff0").style.display = (sw>0)? "none" : "inline-block";
	elm("stream0" ).style.display = (sw>0)? "none" : "inline-block";
	sw = (sw>0)? "inline-block" : "none";
	elm("connect0").style.display = sw;
	elm("stream1" ).style.display = sw;
	elm("poweroff").style.display = sw;
	elm("takeoff1").style.display = sw;
}
function offline(id) {
	return document.getElementById(id);
}
window.onload = function() {
	let canvas = elm('video-canvas');
	let ctx = canvas.getContext("2d");
	//let url = 'ws://' + server + ':8081/stream';
	//let player = new JSMpeg.Player(url, {canvas: canvas, audio: false});

	ws1 = new WebSocket('ws://' + server + ':8080');
	ws2 = new WebSocket('ws://' + server + ':8081');
	ws2.onmessage = function (event) {
		if(event.data instanceof Blob) {
   			const img = new Image();
			img.onload = (ev) => {
				URL.revokeObjectURL(ev.target.src);
				ctx.drawImage(ev.target, 0, 0);
			};
			img.src = URL.createObjectURL(event.data);
			return;
		}
	};
	ws1.onmessage = function (event) {
		if(event.data == "") {
			return;
		}
		if(event.data == "command ok") {
			return;
		}
		if(event.data == "connect ok") {
			connected(1);
		} else
		if(event.data == "disconnect ok") {
			elm('video-canvas').style.display = "none";
			elm("img1").className = "";
			elm("img2").className = "";
			elm("img3").className = "";
			elm("img4").className = "";
			connected(0);
			camera = false;
			tello = false;
		} else
		if(event.data == "streamon ok") {
			elm('video-canvas').style.display = "inline-block";
			camera = true;
		} else
		if(event.data == "streamoff ok") {
			elm('video-canvas').style.display = "none";
			camera = false;
		} else
		if(event.data == "takeoff ok") {
			flyani(1);
		} else
		if(event.data.indexOf("takeoff error") >= 0) {
			flyani(-1);
		} else
		if(event.data == "land ok") {
			flyani(-1);
		} else
		if(event.data == "emergency ok") {
			flyani(-1);
		}
		let json = {};
		let er=1;
		try {
			eval("json = {" +  event.data.replace(/;/g, ",") + "};");
			er=0;
		} catch(e) { }
		for(let name in json) {
			let e = elm(name);
			if(e == null)
				 er++;
			else {
				if(e.innerHTML == "" + json[name]) {
					e.style.backgroundColor = "#fff0";
				} else {
					e.style.backgroundColor = "#ff88";
					e.innerHTML = "" + json[name];
				}
				if(name == "bat") {
					bat = parseInt(json[name]);
					let c = elm("batc");
					c.style.width = "calc(" + bat + "% - 1px)";
					if(bat >= 50) {
						c.style.background = "#0f05";

					} else if(bat >= 25) {
						c.style.background = "#f905";
					} else {
						c.style.background = "#f005";
						e.style.color = "red";
					}
					flyani(ani);
				}
			}
		}
		if(er>0) {
			let e = elm('tello-message');
			if(line < 50) {
				line++;
				e.innerHTML += "<div>" + event.data + "</div>";
			} else {
				let p = e.innerHTML.indexOf("</div>");
				e.innerHTML = e.innerHTML.substr(p+6)
				+ "<div>" + event.data + "</div>";
			}
			e.scrollBy(0, 9999);
		}
	};

	elm("s1").style.background = "powderblue";
	elm("d1").style.background = "powderblue";

	document.body.addEventListener("keydown", keydown);
	document.body.addEventListener("keyup", keyup);
}

function speedx(e) {
	speed = parseInt(e.id.substring(1));
	elm("s1").style.background = "";
	elm("s2").style.background = "";
	elm("s3").style.background = "";
	elm("s" + speed).style.background = "powderblue";
	if(speed == 1) speed = 10;
	else if(speed == 2) speed = 50;
	else speed = 100;
	ws1.send("speed " + speed);
}
function distx(e) {
	dist = parseInt(e.id.substring(1));
	elm("d1").style.background = "";
	elm("d2").style.background = "";
	elm("d3").style.background = "";
	elm("d" + dist).style.background = "powderblue";
	if(dist == 1) dist = 20;
	else if(dist == 2) dist = 50;
	else dist = 0;
}

var vkey = 0;
function keydown(event) {
	_keydown(event.code);
}
function _keydown(code) {
	if(vkey != 0) return;
	if(code == "KeyW") {
		vkey = code;
		elm("ru").style.display = "inline-block";
		if(dist == 0)
			ws1.send("rc 0 0 30 0");
		else {
			let cmd = "up " + dist;
			ws1.send(cmd);
		}
	}
	if(code == "KeyS") {
		vkey = code;
		elm("rd").style.display = "inline-block";
		if(dist == 0)
			ws1.send("rc 0 0 -30 0");
		else {
			let cmd = "down " + dist;
			ws1.send(cmd);
		}
	}
	if(code == "KeyA") {
		vkey = code;
		elm("rl").style.display = "inline-block";
		if(dist == 0)
			ws1.send("rc 0 0 0 -30");
		else {
			let cmd = "ccw 30";
			ws1.send(cmd);
		}
	}
	if(code == "KeyD") {
		vkey = code;
		elm("rr").style.display = "inline-block";
		if(dist == 0)
			ws1.send("rc 0 0 0 30");
		else {
			let cmd = "cw 30";
			ws1.send(cmd);
		}
	}
	if(code == "ArrowUp") {
		vkey = code;
		elm("g0").style.display = "inline-block";
		if(dist == 0)
			ws1.send("rc 0 30 0 0");
		else {
			let cmd = "forward " + dist;
			ws1.send(cmd);
		}
	}
	if(code == "ArrowDown") {
		vkey = code;
		elm("g6").style.display = "inline-block";
		if(dist == 0)
			ws1.send("rc 0 -30 0 0");
		else {
			let cmd = "back " + dist;
			ws1.send(cmd);
		}
	}
	if(code == "ArrowLeft") {
		vkey = code;
		elm("g9").style.display = "inline-block";
		if(dist == 0)
			ws1.send("rc -30 0 0 0");
		else {
			let cmd = "left " + dist;
			ws1.send(cmd);
		}
	}
	if(code == "ArrowRight") {
		vkey = code;
		elm("g3").style.display = "inline-block";
		if(dist == 0)
			ws1.send("rc 30 0 0 0");
		else {
			let cmd = "right " + dist;
			ws1.send(cmd);
		}
	}
}
function keyup(event) {
	_keyup(event.code);
}
function _keyup(code) {
	if(dist == 0) {
		ws1.send("rc 0 0 0 0");
	}
	if(vkey == 0) return;
	if(vkey != code) return;
	vkey = 0;
	if(code == "KeyW") {
		elm("ru").style.display = "none";
	}
	if(code == "KeyS") {
		elm("rd").style.display = "none";
	}
	if(code == "KeyA") {
		elm("rl").style.display = "none";
	}
	if(code == "KeyD") {
		elm("rr").style.display = "none";
	}
	if(code == "ArrowUp") {
		elm("g0").style.display = "none";
	}
	if(code == "ArrowDown") {
		elm("g6").style.display = "none";
	}
	if(code == "ArrowLeft") {
		elm("g9").style.display = "none";
	}
	if(code == "ArrowRight") {
		elm("g3").style.display = "none";
	}
}

function connect(sw) {
	if(sw > 0) {
		ws1.send("connect");
		return;
	}
	ws1.send("disconnect");
}
function connected(sw) {
	if(sw > 0) {
		elm("connect1").style.display = "none";
		elm("connect0").style.display = "inline-block";
		elm("stream0" ).style.display = "inline-block";
		elm("stream1" ).style.display = "inline-block";
		elm("takeoff1").style.display = "inline-block";
		elm("takeoff0").style.display = "inline-block";
		elm("poweroff").style.display = "inline-block";
		//elm("table3").style.display = "inline-block";
		//elm("tableG").style.display = "inline-block";
		tello = true;
		ws1.send("speed " + speed);
		return;
	}
	elm("connect1").style.display = "inline-block";
	elm("connect0").style.display = "none";
	elm("stream0" ).style.display = "none";
	elm("stream1" ).style.display = "none";
	elm("takeoff1").style.display = "none";
	elm("takeoff0").style.display = "none";
	elm("poweroff").style.display = "none";
	//elm("table3").style.display = "none";
	//elm("tableG").style.display = "none";
	tello = false;
	camera = false;
}
function stream(sw) {
	if(sw > 0) {
		ws1.send("streamon");
		return;
	}
	ws1.send("streamoff");
}
function takeoff(sw) {
	if(sw > 0) {
		flyani(0);
		ws1.send("takeoff");
		return;
	}
	if(sw < 0) {
		ws1.send("emergency");
		return;
	}
	ws1.send("land");
}
function flyani(sw) {
	ani = sw;
	if(sw < 0) {
		elm("img1").className = "";
		elm("img2").className = "";
		elm("img3").className = "";
		elm("img4").className = "";
		return;
	}
	let c = (bat>=50)? "g" : (bat>=25)? "y" : "r";
	elm("img1").className = c + sw;
	elm("img2").className = c + sw;
	elm("img3").className = c + sw;
	elm("img4").className = c + sw;
}
function stop() {
	ws1.send("stop");
}

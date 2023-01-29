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
	ws1.onerror = function(e) {
		xlog(e.type + " " + e.target.url);
		elm("connect1").disabled = "disabled";
	};
	ws2 = new WebSocket('ws://' + server + ':8081');
	ws2.onerror = function(e) { xlog(e.type + " " + e.target.url); };
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
		event.data = state(event.data);
		if(event.data == "") return;

		if(event.data == "command ok") {
			return;
		}
		if(event.data == "connect ok") {
			connected(1);
			return;
		}
		if(event.data == "disconnect ok") {
			elm('video-canvas').style.display = "none";
			elm("img1").className = "";
			elm("img2").className = "";
			elm("img3").className = "";
			elm("img4").className = "";
			connected(0);
			camera = false;
			tello = false;
			return;
		}
		if(event.data == "streamon ok") {
			elm('video-canvas').style.display = "inline-block";
			camera = true;
			return;
		}
		if(event.data == "streamoff ok") {
			elm('video-canvas').style.display = "none";
			camera = false;
			return;
		}
		if(event.data.startsWith("takeoff")) {
			elm("takeoff1").style.background = "";
			if(event.data == "takeoff ok") {
				flyani(1);
				return;
			}
			flyani(-1);
			return;
		}
		if(event.data.startsWith("land")) {
			elm("takeoff0").style.background = "";
			flyani(-1);
			return;
		}
		if(event.data.startsWith("emergency")) {
			elm("takeoff0").style.background = "";
			flyani(-1);
			return;
		}
	};

	elm("s1").style.background = "powderblue";
	elm("d1").style.background = "powderblue";

	document.body.addEventListener("keydown", keydown);
	document.body.addEventListener("keyup", keyup);
}

function state(m) {
	m = m.replace(/;/g, ",");
	let json = {};
	let er = m;
	try {
		eval("json = {" +  m + "};");
		er = "";
	} catch(e) {
	}
	for(let name in json) {
		let e = elm(name);
		if(e == null)
			 er += " " + name + ":" +json[name];
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
	err = err.trim();
	xlog(err);
	return err;
}
function xlog(err) {
	if(err == "") return;
	let e = elm('tello-message');
	if(line < 50) {
		line++;
		e.innerHTML += "<div>" + err + "</div>";
	} else {
		let p = e.innerHTML.indexOf("</div>");
		e.innerHTML = e.innerHTML.substr(p+6)
		+ "<div>" + err + "</div>";
	}
	e.scrollBy(0, 9999);
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
	ws1send("speed " + speed);
}
function distx(e) {
	let d = parseInt(e.id.substring(1));
	dist_(d);
}
function dist_(d) {
	elm("d1").style.background = "";
	elm("d2").style.background = "";
	elm("d3").style.background = "";
	elm("d" + d).style.background = "powderblue";
	if(d == 1) dist = 20;
	else if(d == 2) dist = 50;
	else dist = 0;
}

function connect(sw) {
	if(sw > 0) {
		ws1send("connect");
		return;
	}
	ws1send("disconnect");
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
		ws1send("speed " + speed);
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
		ws1send("streamon");
		return;
	}
	ws1send("streamoff");
}
function takeoff(sw) {
	if(sw > 0) {
		if(ws1send("takeoff")) {
			elm("takeoff1").style.background = "pink";
			flyani(0);
		}
		return;
	}
	if(sw < 0) {
		if(ws1send("emergency")) {
			elm("poweroff").style.background = "pink";
			setTimeout(function() {
				elm("poweroff").style.background = "";
			}, 1000);
		}
		return;
	}
	if(ws1send("land")) {
		elm("takeoff0").style.background = "pink";
	}
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
	//ws1send("stop");
	ws1send("rc 0 0 0 0");
}
function ws1send(s) {
	try {
		if(ws1.readyState == 1/*OPEN*/) {
			ws1.send(s);
			return true;
		}
	} catch(e) {
		// NONE
	}
	return false;
}

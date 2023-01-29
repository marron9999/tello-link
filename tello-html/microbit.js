var MBIT1 = null;
var MBIT2 = null;
var UART1 = {};
var UART2 = {};
async function onmbitble1() {
	dist_(3);
	let e = elm("mbitble1");
	if(e.style.background != "pink") {
		initBLE1();
		//MBIT1.LOG('MBIT1.connect("UART", UART1)');
		await MBIT1.connect("UART", UART1);
		if (MBIT1._device != null) {
			e.style.background = "pink";
			xlog("connect " + MBIT1._device_name);
			e = elm("mbitble2");
			e.disabled = "";
			e.children[0].style.filter = "opacity(1)";
		}
	} else {
		e.style.background = "";
		e = elm("mbitble2");
		e.disabled = "disabled";
		e.children[0].style.filter = "opacity(0.3)";
		MBIT1.disconnect();
		MBIT1 = null;
		if(MBIT2 != null) {
			MBIT2.disconnect();
			MBIT2 = null;
		}
	}
}
async function onmbitble2() {
	dist_(3);
	let e = elm("mbitble2");
	if(e.style.background != "pink") {
		initBLE2();
		//MBIT1.LOG('MBIT2.connect("UART", UART2)');
		await MBIT2.connect("UART", UART2);
		if (MBIT2._device != null) {
			e.style.background = "pink";
			xlog("connect " + MBIT2._device_name);
		}
	} else {
		e.style.background = "";
		MBIT2.disconnect();
		MBIT2 = null;
	}
}

let pm = 0;
let _tof = "";
let _d1x = "";
let _d1y = "";
let _d2x = "";
let _d2y = "";
let _ble = "";
let initBLE1 = function () {
	MBIT1 = newBLE();
	UART1["TX"] = function(event) {
		let raw = UART1.text(event);
		raw = raw.replace(/\r/g, "");
		raw = raw.replace(/\n/g, "");
		let d = raw.split(" ");
		if(d[0] == "X") {
			d[1] = parseInt(parseFloat(d[1]) * 100) / 100;
			//LOG0(d);
			if(d[1] <= -0.25) {
				//E("x").style.transform = "translate(-25px, 0px)";
				if(_d1x == "ArrowLeft") return;
				_keydown(_d1x = "ArrowLeft");
			} else if(d[1] >= 0.25) {
				//E("x").style.transform = "translate(25px, 0px)";
				if(_d1x == "ArrowRight") return;
				_keydown(_d1x = "ArrowRight");
			} else {
				//E("x").style.transform = "translate(0px, 0px)";
				if(_d1x == "") return;
				_keyup(_d1x);
				_d1x = "";
			}
			return;
		}
		if(d[0] == "Y") {
			d[1] = parseInt(parseFloat(d[1]) * 100) / 100;
			//LOG0(d);
			if(d[1] <= -0.25) {
				//E("y").style.transform = "rotate(-30deg)";
				if(_d1y == "ArrowUp") return;
				_keydown(_d1y = "ArrowUp");
			} else if(d[1] >= 0.25) {
				//E("y").style.transform = "rotate(30deg)";
				if(_d1y == "ArrowDown") return;
				_keydown(_d1y = "ArrowDown");
			} else {
				//E("y").style.transform = "rotate(0deg)";
				if(_d1y == "") return;
				_keyup(_d1y);
				_d1y = "";
			}
			return;
		}
		if(d[0] == "Z") {
			d[1] = parseInt(parseFloat(d[1]) * 100) / 100;
			//LOG0(d);
			return;
		}
		if(d[0] == "A") {
			//LOG0(d);
			if(d[1] == "1") {
				//E("r").style.transform = "rotate(-45deg)";
				if(_ble == "KeyA") return;
				_keydown(_ble = "KeyA");
			} else {
				//E("r").style.transform = "rotate(0deg)";
				if(_ble == "") return;
				_keyup(_ble);
				_ble = "";
			}
			return;
		}
		if(d[0] == "B") {
			//LOG0(d);
			if(d[1] == "1") {
				//E("r").style.transform = "rotate(45deg)";
				if(_ble == "KeyD") return;
				_keydown(_ble = "KeyD");
			} else {
				//E("r").style.transform = "rotate(0deg)";
				if(_ble == "") return;
				_keyup(_ble);
				_ble = "";
			}
			return;
		}
		if(d[0] == "LOGO") {
			//LOG0(d);
			if(d[1] == "1") {
				pm = (pm + 1) % 2;
				if(pm == 1) {
					//E("m").innerHTML = "モーター オン";
					//E("q").className = "a";
					let e = elm(_tof = "takeoff1");
					e.style.background = "pink";
					e.click();
					setTimeout(function() {
						elm(_tof).style.background = "";
						_tof = "";
					}, 1000);
				} else {
					//E("m").innerHTML = "モーター オフ";
					//E("q").className = "";
					let e = elm(_tof = "takeoff0");
					e.style.background = "pink";
					e.click();
					setTimeout(function() {
						elm(_tof).style.background = "";
						_tof = "";
					}, 1000);
				}
			}
			pm != d[1];
			return;
		}
		if(d[0] == "P1") {
			//LOG0(d);
			if(d[1] == "1") {
				//E("z").style.transform = "translate(0px, -25px)";
				if(_ble == "KeyW") return;
				_keydown(_ble = "KeyW");
			} else {
				//E("z").style.transform = "translate(0px, 0px)";
				if(_ble == "") return;
				_keyup(_ble);
				_ble = "";
			}
			return;
		}
		if(d[0] == "P2") {
			//LOG0(d);
			if(d[1] == "1") {
				//E("z").style.transform = "translate(0px, 25px)";
				if(_ble == "KeyS") return;
				_keydown(_ble = "KeyS");
			} else {
				//E("z").style.transform = "translate(0px, 0px)";
				if(_ble == "") return;
				_keyup(_ble);
				_ble = "";
			}
			return;
		}
	};
}
initBLE2 = function () {
	MBIT2 = newBLE();
	UART2["TX"] = function(event) {
		let raw = UART2.text(event);
		raw = raw.replace(/\r/g, "");
		raw = raw.replace(/\n/g, "");
		let d = raw.split(" ");
		if(d[0] == "X") {
			d[1] = parseInt(parseFloat(d[1]) * 100) / 100;
			if(d[1] <= -0.25) {
				//d = ["B", "0"];
				//LOG0(d);
				//d = ["A", "1"];
				//LOG0(d);
				//E("r").style.transform = "rotate(-45deg)";
				if(_d2x == "KeyD") return;
				_keydown(_d2x = "KeyD");
			} else if(d[1] >= 0.25) {
				//d = ["A", "0"];
				//LOG0(d);
				//d = ["B", "1"];
				//LOG0(d);
				//E("r").style.transform = "rotate(45deg)";
				if(_d2x == "KeyA") return;
				_keydown(_d2x = "KeyA");
			} else {
				//d = ["A", "0"];
				//LOG0(d);
				//d = ["B", "0"];
				//LOG0(d);
				//E("r").style.transform = "rotate(0deg)";
				if(_d2x == "") return;
				_keyup(_d2x);
				_d2x = "";
			}
			return;
		}
		if(d[0] == "Y") {
			d[1] = parseInt(parseFloat(d[1]) * 100) / 100;
			if(d[1] <= -0.25) {
				//d = ["P1", "0"];
				//LOG0(d);
				//d = ["P2", "1"];
				//LOG0(d);
				//E("z").style.transform = "translate(0px, 25px)";
				if(_d2y == "KeyW") return;
				_keydown(_d2y = "KeyW");
			}
			else if(d[1] >= 0.25) {
				//d = ["P2", "0"];
				//LOG0(d);
				//d = ["P1", "1"];
				//LOG0(d);
				//E("z").style.transform = "translate(0px, -25px)";
				if(_d2y == "KeyS") return;
				_keydown(_d2y = "KeyS");
			} else {
				//d = ["P1", "0"];
				//LOG0(d);
				//d = ["P2", "0"];
				//LOG0(d);
				//E("z").style.transform = "translate(0px, 0px)";
				if(_d2y == "") return;
				_keyup(_d2y);
				_d2y = "";
			}
			return;
		}
		if(d[0] == "LOGO") {
			LOG0(d);
			if(d[1] == "1") {
				pm = (pm + 1) % 2;
				if(pm == 1) {
					//E("m").innerHTML = "モーター オン";
					//E("q").className = "a";
					let e = elm(_tof = "takeoff1");
					e.style.background = "pink";
					e.click();
					setTimeout(function() {
						elm(_toff).style.background = null;
						_toff = "";
					}, 1000);
				} else {
					//E("m").innerHTML = "モーター オフ";
					//E("q").className = "";
					let e = elm(_tof = "takeoff0");
					e.style.background = "pink";
					e.click();
					setTimeout(function() {
						elm(_tof).style.background = null;
						_tof = "";
					}, 1000);
				}
			}
			pm != d[1];
			return;
		}
	};
}

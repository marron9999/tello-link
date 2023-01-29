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
		MBIT1.connected = function () {
			let e = elm("mbitble1");
			e.style.background = "pink";
			xlog("connect " + MBIT1._device_name);
			e = elm("mbitble2");
			e.disabled = "";
			e.children[0].style.filter = "opacity(1)";
		};
		MBIT1.disconnected = function () {
			let e = elm("mbitble1");
			e.style.background = "";
			e = elm("mbitble2");
			e.disabled = "disabled";
			e.children[0].style.filter = "opacity(0.3)";
			xlog("disconnect microbit 1");
			MBIT1 = null;
			if(MBIT2 != null) {
				MBIT2.disconnect();
				MBIT2 = null;
			}
		};
		await MBIT1.connect("UART", UART1);
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
		MBIT1.connected = function () {
			let e = elm("mbitble2");
			e.style.background = "pink";
			xlog("connect " + MBIT2._device_name);
		};
		MBIT1.disconnected = function () {
			let e = elm("mbitble2");
			e.style.background = "";
			xlog("disconnect microbit 2");
			MBIT2 = null;
		};
		await MBIT2.connect("UART", UART2);
	} else {
		e.style.background = "";
		MBIT2.disconnect();
		MBIT2 = null;
	}
}

let _x = 0;
let _y = 0;

function xy() {
	if(_x <= -0.25) {
		//E("x").style.transform = "translate(-25px, 0px)";
		if(_y <= -0.25) {
			if(_d1x == "ArrowLeftUp") return;
			_keyup(_d1x);
			_keydown(_d1x = "ArrowLeftUp");
		} else if(_y >= 0.25) {
			if(_d1x == "ArrowLeftDown") return;
			_keyup(_d1x);
			_keydown(_d1x = "ArrowLeftDown");
		} else {
			if(_d1x == "ArrowLeft") return;
			_keyup(_d1x);
			_keydown(_d1x = "ArrowLeft");
		}
	} else if(_x >= 0.25) {
		//E("x").style.transform = "translate(25px, 0px)";
		if(_y <= -0.25) {
			if(_d1x == "ArrowRightUp") return;
			_keyup(_d1x);
			_keydown(_d1x = "ArrowRightUp");
		} else if(_y >= 0.25) {
			if(_d1x == "ArrowRightDown") return;
			_keyup(_d1x);
			_keydown(_d1x = "ArrowRightDown");
		} else {
			if(_d1x == "ArrowRight") return;
			_keyup(_d1x);
			_keydown(_d1x = "ArrowRight");
		}
	} else if(_y <= -0.25) {
		if(_d1x == "ArrowUp") return;
		_keyup(_d1x);
		_keydown(_d1x = "ArrowUp");
	} else if(_y >= 0.25) {
		if(_d1x == "ArrowDown") return;
		_keyup(_d1x);
		_keydown(_d1x = "ArrowDown");
	} else {
		//E("x").style.transform = "translate(0px, 0px)";
		if(_d1x == "") return;
		_keyup(_d1x);
		_d1x = "";
	}
}

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
			_x = parseInt(parseFloat(d[1]) * 100) / 100;
			//LOG0(d);
			xy();
			return;
		}
		if(d[0] == "Y") {
			_y = parseInt(parseFloat(d[1]) * 100) / 100;
			//LOG0(d);
			xy();
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
		if(d[0] == "LOGO") {
			logo(d);
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
			logo(d);
			return;
		}
	};
}

function logo(d) {
	//LOG0(d);
	if(d[1] == "1") {
		if(ani < 0) {
			//E("m").innerHTML = "モーター オン";
			//E("q").className = "a";
			takeoff(1);
		} else {
			//E("m").innerHTML = "モーター オフ";
			//E("q").className = "";
			takeoff(0);
		}
	}
}


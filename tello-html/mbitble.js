function E(id) {
	return document.getElementById(id);
}
//function LOG(s) {
//	MBITBLE.LOG(s);
//}

function _MBITBLE() {
let ble = {
//	LOGMAX : 11,
//	logged : 0,
//	LOG: function (s) {
//		let e = E("log");
//		if(e == null) return;
//		e.innerHTML += "<div>" + s + "</div>";
//		this.logged++;
//		if(this.logged > this.LOGMAX) {
//			this.logged--;
//			let i = e.innerHTML.indexOf("</div>");
//			e.innerHTML = e.innerHTML.substr(i+6);
//		}
//		e.scroll(0, 9999);
//	},
//	LOG2 : this.LOG,

	_log: "",
	_log2: "",
	_error: "",
	log: function (text) { _log = text; },
	log2: function (text) {_log2 = text; },
	error: function (text) { _error = text; },
	connected: function () { },
	disconnected: function () { },

	verbose: true,

	connect: async function (names, targets) {
		this._targets = {};
		if( ! Array.isArray(names)) {
			names = [names];
			targets = [targets];
		}
		try {
			let services = [];
			for(let i=0; i<names.length; i++) {
				services[i] = MBITUUID[names[i]].UUID;
				this._targets[names[i]] = targets[i];
			}
			let option = {
				acceptAllDevices: false,
				filters: [
					{ services: services}, // <- 重要
					{ namePrefix: this._device_type },
				]
			};
			this._device = await navigator.bluetooth.requestDevice(option);
			this._server = await this._device.gatt.connect();
			this._device_name = this._device.name;
			this._device.MBITBLE = this;
			this._device.ongattserverdisconnected = function() {
				this.MBITBLE.log2("Disconnected"); 
				for(let name in MBITBLE._targets) {
					this.MBITBLE._targets[name].primary = null;
				}
				this.MBITBLE.init();
				this.MBITBLE.disconnected();
			};
			for(let i=0; i<names.length; i++) {
				await this.service(names[i], targets[i]);
			}
			this.log2("Connected: " + this._device_name);
			this.connected();
		} catch (error) {
			this.init();
			this.error(error);
		}
	},
	disconnect: function () {
		if (this._device == null) {
			this.error("disconnect: " + "device is null");
			return;
		}
		this._device.gatt.disconnect();
		this.init();
	},
	init: function () {
		this._device = null;
		this._server = null;
		this._device_name = null;
		this._targets = {};
	},
	_device_type: "BBC micro:bit",
	_device_name: null,
	_device: null,
	_server: null,
	_targets: {},
	_encoder: new TextEncoder('utf-8'),
	_decoder: new TextDecoder('utf-8'),

	service: async function (name, target) {
		if(target == undefined
		|| target == null) target = {};
		target.primary = null;
		target._ = {};

		try {
			let primary = await this._server.getPrimaryService(MBITUUID[name].UUID);
			if(this.verbose) {
				this.log("Primary: " + name + " Service");
				this.log(MBITUUID[name].UUID);
			}
			if (primary == null) {
				MBITBLE.error("characteristic: " + "primary is null");
				return;
			}
			primary.MBITBLE = this;
			let characteristic = async function (primary, uuid, desc, callback) {
				try {
					let characteristic = await primary.getCharacteristic(uuid);
					if (characteristic != null
					&& callback != undefined
					&& callback != null) {
						characteristic.addEventListener("characteristicvaluechanged", callback);
						characteristic.startNotifications();
						if(primary.MBITBLE.verbose) {
							primary.MBITBLE.log("Characteristic: " + desc + " + listener");
						}
					} else {
						if(primary.MBITBLE.verbose) {
							primary.MBITBLE.log("Characteristic: " + desc);
						}
					}
					if(primary.MBITBLE.verbose) {
						primary.MBITBLE.log(uuid);
					}
					return characteristic;
				} catch (error) {
					primary.MBITBLE.error(error);
				}
				return;
			};
			for(let func in MBITUUID[name].SERVICE) {
				target._[func] = await characteristic(
					primary, MBITUUID[name].SERVICE[func], func,
					(target[func] == undefined)? null : target[func]);
			}

			target._mbitble = this;
			target.text = function (event) {
				return this._mbitble._decoder.decode(event.target.value);
			};
			target.value = function (event) {
				return event.target.value;
			};
			target.int8 = function (event, n) {
				return event.target.value.getInt8(n);
			};
			target.uint8 = function (event, n) {
				return event.target.value.getUint8(n);
			};
			target.int16 = function (event, n) {
				return event.target.value.getInt16(n, true);
			};
			target.uint16 = function (event, n) {
				return event.target.value.getUint16(n, true);
			};
			target.write_data = async function(name, data) {
				try {
					await this._[name].writeValue(data);
				} catch (error) {
					this._mbitble.error(error);
				}
			};
			target.write_text = async function (name, text) {
				let buffer = this._mbitble._encoder.encode(text);
				this.write_data(name, buffer);
			};
			target.read_data = async function (name) {
				try {
					let array = await this._[name].readValue();
					if(array != null) {
						if(array.length == undefined)
							array.length = array.byteLength;
					}
					return array;
				} catch(error) {
					this._mbitble.error(error);
				}
				return;
			};
			target.read_text = async function (name) {
				let event = await this.read_data(name);
				if(event == null) return null;
				return this.text(event);
			};
			target.primary = primary;
		} catch (error) {
			this.error(error);
		}
	}
	//disconnectBLE : async function () {
	//	await MBITBLE.disconnect();
	//},
	//connectBLE : null
};
ble.LOG2 = ble.LOG;
return ble;
}

var connectBLE = async function() {
	await MBITBLE.connect();
}
var disconnectBLE = async function() {
	await MBITBLE.disconnect();
}

var MBITBLE = _MBITBLE();
var initBLE = async function () {
	MBITBLE.logged = 0;
	let e = E("log");
	if(e != null) e.innerHTML = "";
};
var newBLE = function () {
	let ble = _MBITBLE();
	ble.logged = 0;
	return ble;
};

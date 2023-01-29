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
		return;
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
		return;
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
		return;
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
		return;
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
		return;
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
		return;
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
		return;
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
		return;
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
		return;
	}
	if(code == "KeyS") {
		elm("rd").style.display = "none";
		return;
	}
	if(code == "KeyA") {
		elm("rl").style.display = "none";
		return;
	}
	if(code == "KeyD") {
		elm("rr").style.display = "none";
		return;
	}
	if(code == "ArrowUp") {
		elm("g0").style.display = "none";
		return;
	}
	if(code == "ArrowDown") {
		elm("g6").style.display = "none";
		return;
	}
	if(code == "ArrowLeft") {
		elm("g9").style.display = "none";
		return;
	}
	if(code == "ArrowRight") {
		elm("g3").style.display = "none";
		return;
	}
}


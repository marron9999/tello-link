let repeat = true;
SpeechRecognition = webkitSpeechRecognition || SpeechRecognition;
const recognition = new SpeechRecognition();
recognition.interimResults = true;
//recognition.continuous = true;
recognition.maxAlternatives = 1;
//recognition.lang = 'en-US';
function beep() {
    var snd = new Audio("data:audio/wav;base64,UklGRvgUAABXQVZFZm10IBAAAAABAAIARKwAABCxAgAEABAAZGF0YdQUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA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");  
    snd.play();
}
recognition.onaudioend = (event) => {
	//elm("out" + out).innerHTML += "<br>- audio end -";
}
recognition.onaudiostart = (event) => {
	beep();
	//elm("out" + out).innerHTML += "<br>- audio start -";
	//elm("b").innerHTML = C.Z;
}
let prev = "";
function push(id) {
	//elm(id).style.display = "inline-block";
	if(id == "g0") id = "ArrowUp";
	else if(id == "g3") id = "ArrowRight";
	else if(id == "g6") id = "ArrowDown";
	else if(id == "g9") id = "ArrowLeft";
	else if(id == "ru") id = "KeyW";
	else if(id == "rd") id = "KeyS";
	else if(id == "rl") id = "KeyA";
	else if(id == "rr") id = "KeyD";
	else id = null;
	if(prev == id) return;
	prev = id;
	if(id != null) {
		_keydown(id);
		prev = "";
		setTimeout(function() {
			//elm(id).style.display = "none";
			_keyup(id);
		}, 1000);
	}
}
function push1(id) {
	if(!tello) return;
	if(prev == id) return;
	prev = id;
	elm(id).style.background = "pink";
	elm(id).click();
	setTimeout(function() {
		elm(id).style.background = "";
		prev = "";
	}, 1000);
}
function push2(id) {
	if(prev == id) return;
	prev = id;
	elm(id).style.background = "pink";
	elm(id).click();
	setTimeout(function() {
		elm(id).style.background = "";
		prev = "";
	}, 1000);
}
function stopx() {
	if(prev == "stop") return;
	prev = "stop";
	stop();
	setTimeout(function() {
		prev = "";
	}, 1000);
}
function recparse(s) {
	if(s.indexOf("前") >= 0) push("g0");
	if(s.indexOf("後") >= 0) push("g6");
	if(s.indexOf("バッ") >= 0) push("g6");
	if(s.indexOf("ック") >= 0) push("g6");
	if(s.indexOf("右") >= 0) push("g3");
	if(s.indexOf("左") >= 0) push("g9");
	if(s.indexOf("上") >= 0) push("ru");
	if(s.indexOf("アッ") >= 0) push("ru");
	if(s.indexOf("ップ") >= 0) push("ru");
	if(s.indexOf("下") >= 0) push("rd");
	if(s.indexOf("ダウ") >= 0) push("rd");
	if(s.indexOf("ウン") >= 0) push("rd");
	if(s.indexOf("みぎ") >= 0) push("g3");
	if(s.indexOf("ひだ") >= 0) push("g9");
	if(s.indexOf("だり") >= 0) push("g9");
	if(s.indexOf("まえ") >= 0) push("g0");
	if(s.indexOf("うし") >= 0) push("g6");
	if(s.indexOf("しろ") >= 0) push("g6");
	if(s.indexOf("フト") >= 0) push("rl");
	if(s.indexOf("レフ") >= 0) push("rl");
	if(s.indexOf("ライ") >= 0) push("rr");
	if(s.indexOf("イト") >= 0) push("rr");
	if(s.indexOf("着") >= 0) push1("takeoff0");
	if(s.indexOf("離") >= 0) push1("takeoff1");
	if(s.indexOf("オフ") >= 0) push1("takeoff1");
	if(s.indexOf("ラン") >= 0) push1("takeoff0");
	if(s.indexOf("ンド") >= 0) push1("takeoff0");
	if(s.indexOf("何") >= 0) push1("takeoff0");
	if(s.indexOf("度") >= 0) push1("takeoff0");
	if(s.indexOf("土") >= 0) push1("takeoff0");
	if(s.indexOf("接続") >= 0) push2("connect1");
	if(s.indexOf("切断") >= 0) push2("connect0");
	if(s.indexOf("接") >= 0) push2("connect1");
	if(s.indexOf("続") >= 0) push2("connect1");
	if(s.indexOf("切") >= 0) push2("connect0");
	if(s.indexOf("断") >= 0) push2("connect0");
	if(s.indexOf("停止") >= 0) stopx();
	if(s.indexOf("停") >= 0) stopx();
	if(s.indexOf("止") >= 0) push3();
	if(s.indexOf("カメ") >= 0) push2("stream1");
	if(s.indexOf("メラ") >= 0) push2("stream1");
	if(s.indexOf("消") >= 0) push2("stream0");
	console.log(s);
}
recognition.onresult = (event) => {
	//elm("out" + out).innerHTML += "<br>";
	for(let i=event.resultIndex; i <event.results.length; i++) {
		if (event.results[i].isFinal) {
			//elm("out" + out).innerHTML += "&lt; ";
		}
		//elm("out" + out).innerHTML += i + ": ";
		for(let j=0; j <event.results[i].length; j++) {
			recparse(event.results[i][j].transcript);
			//elm("out" + out).innerHTML += "'" + event.results[i][j].transcript + "'　";
		}
	}
}
recognition.onend = (event) => {
	//out = (out + 1) % 10;
	//elm("out" + out).innerHTML = "";
	if(repeat) {
		recognition.start();
	}
}
function onvoice() {
	let e = elm("voice");
	if(e.style.background != "pink") {
		repeat = true;
		e.style.background = "pink";
		recognition.start();
	} else {
		repeat = false;
		e.style.background = "";
		recognition.stop();
	}
}
/*
let C = {
	F: "&#x2b61;",
	B: "&#x2b63;",
	R: "&#x2b60;",
	L: "&#x2b62;",
	U: "&#x2ba5;",
	D: "&#x2ba6;",
	RR: "&#x2b6e;",
	LR: "&#x2b6f;",
	Z: "・"
};*/


/**
 * aurora script
 * */
/* external declare */
import './libs/common';
import './md5';
/* Import the exported objects in the script `test` to the `Test` namespace  */
import * as Document from './document';


name = 123456;

/* exported  attributes */
export var length = 100;
//var regex = /gg/;
var result = ReadFile('hello\'\\\n\"world');

var p = a[0] * b[1].scc();

Document.Print('hello');
Document.Version++;

var num, str = Document.tuple(0xff023);
console.log(num);
console.log(str);

var i = 0;
while (true) {
    console.log(i);
    if (i > 5) break;
    i++;
}

while (i < 5) {
    console.log(i);;;;;;;;;;;
    i++;;;;;;;;;;;;;
}

while (i < 15) i++;

if (i < 15) {
}

;;;;;;;;;;;;;;;;;;
for (var i = 0; i < 100; i++) {
    if (i == 66) break;;;;;;;;;;
    if (i > 33 && i < 55)
        continue;
    else
        console.log(i);
}

var fmtString = `load ${num} 

of ${str}`;

/**
 *
 *
 * @param cmd
 */
export function main(cmd /* * block comment */) {
    console.log('hello wrold!');
}

function add(a, b) {
    return (a + b) * (a + b);
}

 add(1, 2);
// yield;
var c, d, r = (
    33 + 66)
    /
    add(
        10_0_0_0 *
        (
            -10 + 0.20_00)
        , // max
        50 + Math.max(
            5, 32 * 0.5
        ) / 20)
    + 3.1415 * 52;

console.log(c);
console.log(d);
console.log(r);

// expression
var len = 50;
var px = len * (-len + add(0x22 + (33 + 0x5), 0xff)) / (6 + 5);
console.log(px);

var age = 22;
var v = (-age + 1.5) - 0x36 * (2.5 / 1.2);
v += 35;
console.log(v);

main('yoyo');

echo.dialog(120)
    .before(Before)
    .after(After)
    .text([
        "====[<$name>]====",
        "bbbbbb",
        "cccccc",
        "dddddd",
    ])
    .floating([
        ui_gif("view", "package://item.wix,12", "0,0"),
        ui_img("view", "package://item.wix,12", "0,0"),
        ui_button("ok", "package://item.wix,13", "15,100"),
        ui_button("cancel", "package://item.wix,14", "200,100"),
    ])
    .position(11, 22)
    .wait(); // 等待关闭

function After() {
}
function Before() {
}



function ok() {
}
function view() {
}
function cancel() {
    say(
        |> * You examine the ${ itemName } closely * 
        |>
        |> <color=${ goldColor } > ${ itemName } < /color>
        |> <i>${ itemRarity } Armor < /i>
        |>
        |> Defense: 45
        |> Weight: 15
        |> Value: 5000 gold
        |>
        |> Special Effects:
        |>   • Fire Resistance + 75 %
        |>   • Cannot be damaged by acid
        |>   • Intimidation bonus against lesser creatures
        |>
        |> "Forged from the scales of the ancient dragon Fyrenthal,
        |> this armor still radiates with magical warmth.The
        |> craftmanship is beyond anything seen in the modern age."
         |>
        |> <Equip/@equipItem> <Back/@showInventory>
    );
}
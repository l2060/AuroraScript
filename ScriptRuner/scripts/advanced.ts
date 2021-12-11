import 'common'

export class base {
    public say(): void {
        console.log('say:');
        for (let i = 0; i < 10; i++) {
            console.log(i);
        }
    }
}

export class demo {
    public constructor() { }
    protected id: number;
    private name: string;
    public foo(fs: number, name: string): boolean {
        this.id = fs;
        this.name = name;
        if (fs && name && fs > 100) {
            return true;
        } else {
            return false;
        }
    }

    public toString(): string {
        return `${this.id}:${this.name}`;
    }
}

export const SkipSelf = '';

[SkipSelf]
export function main(cmd: string): void {

}

var v = 1234;
var n = 'hello';
var cls = new demo();
var result = cls.foo(v, n);
console.log(result);
main('yoyo');
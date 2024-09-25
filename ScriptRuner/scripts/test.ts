//import './main.ts';



export abstract class MyClass extends Window implements Window, Document, Document {
    private static readonly DEFAULT_VALUE: string = '';
    private readonly DEFAULT_VALUE: string = '';

    private _value: string = 'x1';

    public constructor() {
        super();
    }

    public get value(): string {
        return this._value;
    }
    public set value(value: string) {
        this._value = value;
    }


    public index (value: string): string {
        return '';
    }



    public add(): void {
    }

    public item(index: number): string {
        return '';
    }

    public override close(): void {
    }

    public abstract abs(): void;

    public static def(): string {
        return '';
    }
}
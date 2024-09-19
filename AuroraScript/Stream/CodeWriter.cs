using System.Text;

namespace AuroraScript.Stream
{

    public class CustomDisposable : IDisposable
    {
        private readonly Action _disposeAction;

        public CustomDisposable(Action disposeAction)
        {
            _disposeAction = disposeAction ?? throw new ArgumentNullException(nameof(disposeAction));
        }

        public void Dispose()
        {
            _disposeAction();
        }
    }



    public class CodeWriter : StreamWriter
    {
        public Int32 Indented = 0;
        private Boolean IsNewLine = true;

        public CodeWriter(System.IO.Stream stream) : base(stream)
        {

        }
        public CodeWriter(System.IO.Stream stream, Encoding encoding = null, int bufferSize = -1, bool leaveOpen = false) : base(stream, encoding, bufferSize, leaveOpen)
        {

        }

        public IDisposable IncIndented()
        {
            this.Indented++;
            return new CustomDisposable(() =>
            {
                this.Indented--;
            });
        }

        public override void WriteLine()
        {
            WriteIndented();
            base.WriteLine();
            this.IsNewLine = true;
            this.Flush();
        }

        public override void WriteLine(string? value)
        {
            WriteIndented();
            base.WriteLine(value);
            this.IsNewLine = true;
            this.Flush();
        }

        public override void Write(string? value)
        {
            WriteIndented();
            base.Write(value);
            this.IsNewLine = false;
            this.Flush();
        }


        public override void Write(string format, object? arg0, object? arg1)
        {
            WriteIndented();
            base.Write(format, arg0, arg1);
            this.IsNewLine = false;
            this.Flush();
        }


        private void WriteIndented()
        {
            if (Indented <= 0) return;
            if (!this.IsNewLine) return;
            var content = "".PadLeft(Indented * 4, ' ');
            base.Write(content);
            this.IsNewLine = false;
        }




    }
}

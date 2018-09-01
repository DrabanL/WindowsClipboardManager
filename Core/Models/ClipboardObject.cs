namespace RabanSoft.ClipboardExtension.Core.Models {
    public class ClipboardObject {
        public string Format;
        public object Data;

        public ClipboardObject(string format, object data) {
            Format = format;
            Data = data;
        }
    }
}

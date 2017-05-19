namespace FourCDiffServer.Models {
    public class Diff {
        public int Offset { get; set; }
        public int Length { get; set; }

        public Diff(int offset, int length) {
            Offset = offset;
            Length = length;
        }
    }
}
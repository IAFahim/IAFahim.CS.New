# IAFahim.String.Compress

## Description
Implements diverse data compression algorithms. Contains implementations of Huffman coding, Lempel-Ziv variants, arithmetic coding, and Move-To-Front transforms.

## Complexity
Time Complexity is O(N log Sigma) for Huffman encoding, O(N) for MTF, LZ77, and LZ78 algorithms.
Space Complexity is O(Sigma) for Huffman tree and MTF symbols, O(N) for Lempel-Ziv tokens.

## API Signature
```csharp
namespace IAFahim.String.Compress
{
    public static unsafe class Lz78
    {
        public struct Token
        {
            public int Phrase;
            public byte Literal;
        }
        public static int Encode(byte* input, int len, Token* output);
        public static int Decode(Token* input, int count, byte* output);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 4;
    byte* input = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    IAFahim.String.Compress.Lz78.Token* output = (IAFahim.String.Compress.Lz78.Token*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(IAFahim.String.Compress.Lz78.Token));
    try
    {
        input[0] = (byte)'a';
        input[1] = (byte)'b';
        input[2] = (byte)'a';
        input[3] = (byte)'b';
        int tokenCount = IAFahim.String.Compress.Lz78.Encode(input, len, output);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)input);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)output);
    }
}
```

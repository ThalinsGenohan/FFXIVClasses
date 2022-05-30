namespace BlackMage
{
	public static class Elements
	{
		public const byte StackMask  = 0b00011;
		public const byte NoStack    = 0b00;
		public const byte OneStack   = 0b01;
		public const byte FullStack  = 0b10;
		public const byte HeartStack = 0b11;

		public const byte ElementMask      = 0b11100;
		public const byte NoElement        = 0b00000;
		public const byte FireElement      = 0b001 << 2;
		public const byte IceElement       = 0b010 << 2;
		public const byte ParadoxElement   = 0b011 << 2;
		public const byte PolyglotElement  = 0b100 << 2;
		public const byte TransposeElement = 0b101 << 2;
	}
}

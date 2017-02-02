using System;

namespace EvoSim
{
    [Serializable]
    public struct Color
    {
    	public byte A;
    	public byte R;
    	public byte G;
    	public byte B;

    	public Color(int R, int G, int B,int A)
    	{
			this.A = (byte)A;
			this.R = (byte)R;
			this.G = (byte)G;
            this.B = (byte)B;
    	}

    	public static Color FromFloat(float R, float G, float B, float A=1.0f)
    	{
    		return new Color((byte)(R*255),(byte)(B*255),(byte)(B*255),(byte)(A*255));
    	}
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace FVDC
{
	public class StatusStrip
	{
		public static int SizeChanged (int WindowSizeWidth, int BarSizeWidth, bool BarVisible)
		{
			/// 会社名からの表示幅をマイナス
			int	MessageWidth		= WindowSizeWidth - 200 ;	
			
			/// プログレスバー表示時にはバーサイズマイナス
			if (BarVisible)
			{
				MessageWidth		-= BarSizeWidth;
			}
			return MessageWidth;
		}
	}
}

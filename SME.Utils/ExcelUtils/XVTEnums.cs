using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//<author>hungnd8</author>
namespace SME.Utils.ExcelUtils
{
    class XVTEnums
    {
    }
    public enum XLVTBorderStyleValues
    {
        DashDot = 0,
        DashDotDot = 1,
        Dashed = 2,
        Dotted = 3,
        Double = 4,
        Hair = 5,
        Medium = 6,
        MediumDashDot = 7,
        MediumDashDotDot = 8,
        MediumDashed = 9,
        None = 10,
        SlantDashDot = 11,
        Thick = 12,
        Thin = 13,
    }
    public enum XVTBorderIndex
    {
        OutsideBorder, // Border xung quanh
        LeftBorder, // Border trai
        TopBorder, // Border tren
        RightBorder, // Border phai
        BottomBorder, // Border duoi
        DiagonalBorder  // Boder gach cheo
    }
}

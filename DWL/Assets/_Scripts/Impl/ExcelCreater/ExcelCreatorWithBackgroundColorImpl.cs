//using System.Collections.Generic;
//using System.IO;
//using NPOI.HSSF.Util;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
//using UnityEngine;

//public class ExcelCreatorWithBackgroundColorImpl : IExcelCreator
//{
//    //재생성을 막기위한 캐시Dic
//    private Dictionary<int, XSSFCellStyle> styleCache = new Dictionary<int, XSSFCellStyle>();
//    private Dictionary<int, IFont> fontCache = new Dictionary<int, IFont>();

//    public void WriteToExcel(string filePath, List<ExcelCellInfo> cellInfos, int frameCount, int channelCount)
//    {
//        XSSFWorkbook workbook = new XSSFWorkbook();
//        ISheet sheet = workbook.CreateSheet("Data");

//        IRow channelNameRow = sheet.CreateRow(0);
//        channelNameRow.CreateCell(0).SetCellValue("Channel");
//        for (int i = 0; i < channelCount; i++)
//        {
//            channelNameRow.CreateCell(i + 1).SetCellValue($"Ch{i + 1}");
//        }

//        IRow channelPositionRow = sheet.CreateRow(1);
//        channelPositionRow.CreateCell(0).SetCellValue("Pos");
//        for (int i = 0; i < channelCount; i++)
//        {
//            channelPositionRow.CreateCell(i + 1).SetCellValue($"{cellInfos[i].pos.x},{cellInfos[i].pos.y}");
//        }

//        sheet.CreateRow(2);

//        for (int i = 0; i < frameCount; i++)
//        {
//            IRow row = sheet.CreateRow(i + 3);
//            row.CreateCell(0).SetCellValue(i);
//        }

//        foreach (ExcelCellInfo cellInfo in cellInfos)
//        {
//            int rgb = (cellInfo.r << 16) | (cellInfo.g << 8) | cellInfo.b;
//            if (!styleCache.TryGetValue(rgb, out XSSFCellStyle style))
//            {
//                style = (XSSFCellStyle)workbook.CreateCellStyle();
//                XSSFColor color = new XSSFColor(new byte[] { cellInfo.r, cellInfo.g, cellInfo.b });
//                style.SetFillForegroundColor(color);
//                style.FillPattern = FillPattern.SolidForeground;
//                styleCache[rgb] = style;
//            }

//            IRow row = sheet.GetRow(cellInfo.colIndex + 2) ?? sheet.CreateRow(cellInfo.colIndex + 2);
//            ICell cell = row.GetCell(cellInfo.rowIndex + 1) ?? row.CreateCell(cellInfo.rowIndex + 1);
//            cell.SetCellValue(cellInfo.brightness255);
//            cell.CellStyle = style;

//            if (!fontCache.TryGetValue(rgb, out IFont font))
//            {
//                font = workbook.CreateFont();
//                fontCache[rgb] = font;
//            }

//            font.Color = (short)(cellInfo.brightness255 < 85 ? HSSFColor.White.Index : HSSFColor.Black.Index);
//            style.SetFont(font);
//        }

//        using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
//        {
//            workbook.Write(file);
//        }
//    }
//}

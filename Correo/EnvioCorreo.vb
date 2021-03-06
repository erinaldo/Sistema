﻿Imports System.Drawing
Imports System.IO
Imports System.Net.Mail
'Imports System.Net.Mime
''Imports Microsoft.Office.Interop
'Imports Microsoft.Office.Interop.Excel
Imports GemBox.Spreadsheet
Module EnvioCorreo
    Dim pathFile = ""
    Sub Main()
        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY")
        Try
            Dim pathI = Config.RutaInformes
            Dim mes = Date.Today.Month
            Dim anho = Date.Today.Year
            Dim pathImg = Path.Combine(pathI, mes.ToString + "_" + anho.ToString)
            pathImg = pathImg + ".xls"
            pathFile = pathImg
            Dim strCurrency = Chr(34) & "₲" & Chr(34)
            ' pathImg = Path.Combine(pathImg, mes)
            If My.Computer.FileSystem.FileExists(pathImg) Then

                Dim workbook = ExcelFile.Load(pathImg)

                Dim worksheet = workbook.Worksheets(0)
                Dim col = worksheet.Columns(0)

                Dim lastRow = worksheet.Rows.Count - 1

                While lastRow >= 0 And col.Cells(lastRow).ValueType = CellValueType.Null
                    lastRow -= 1
                End While

                lastRow += 1

                Dim daoc As New Consultas
                worksheet.Cells(lastRow, 0).Value = Date.Today.ToShortDateString
                worksheet.Cells(lastRow, 1).Value = daoc.getVentasM2()
                worksheet.Cells(lastRow, 2).Value = daoc.getCorte()
                worksheet.Cells(lastRow, 3).Value = daoc.getProduccion()
                worksheet.Cells(lastRow, 4).Value = daoc.getPendientes()
                worksheet.Cells(lastRow, 5).Value = daoc.getReposicionesDia()

                With worksheet.Cells.GetSubrange("A" + (lastRow + 1).ToString, "F" + (lastRow + 1).ToString)
                    .Style.Font.Size = 11 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(92, 92, 92), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                End With

                worksheet.Cells("H" + (lastRow + 1).ToString).Value = daoc.getEfectivoDia()
                worksheet.Cells("H" + (lastRow + 1).ToString).Style.NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)
                worksheet.Cells("I" + (lastRow + 1).ToString).Value = daoc.getChequesDia()
                worksheet.Cells("I" + (lastRow + 1).ToString).Style.NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)

                worksheet.Cells("J" + (lastRow + 1).ToString).Formula = "=SUM(H" + (lastRow + 1).ToString + ":I" + (lastRow + 1).ToString + ")"
                worksheet.Cells("J" + (lastRow + 1).ToString).Style.NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)
                worksheet.Cells("J" + (lastRow + 1).ToString).Calculate()
                With worksheet.Cells.GetSubrange("H" + (lastRow + 1).ToString, "J" + (lastRow + 1).ToString)
                    .Style.Font.Size = 11 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(92, 92, 92), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                End With

                Dim columnCount = worksheet.CalculateMaxUsedColumns()
                For i As Integer = 0 To 9
                    worksheet.Columns(i).AutoFit(1, worksheet.Rows(1), worksheet.Rows(lastRow))
                Next
                workbook.Save(pathImg)
            Else
                ''-------------------

                Dim workbook As New ExcelFile()
                Dim worksheet = workbook.Worksheets.Add("Correo")

                worksheet.Cells("A1").Value = "MES: " + MonthName(Date.Today.Month) + " -- Año: " + Date.Today.Year.ToString
                With worksheet.Cells("A1")
                    .Style.Font.Size = 16 * 20

                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    '.Style.Font.Weight = ExcelFont.BoldWeight

                End With

                worksheet.Rows(0).Height = 30 * 30
                worksheet.Rows(1).Height = 25 * 20


                Dim cellRange = worksheet.Cells.GetSubrange("A1", "F1")
                cellRange.Merged = True
                cellRange.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(69, 149, 35), Color.Empty)
                cellRange.Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                worksheet.Columns(0).AutoFit()

                With worksheet.Cells("H1")
                    .Value = "Cobros"
                    .Style.Font.Size = 16 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    '.Style.Font.Weight = ExcelFont.BoldWeight
                End With

                With worksheet.Cells.GetSubrange("H1", "J1")
                    .Merged = True
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(69, 149, 35), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)

                End With

                '' TOTALES


                With worksheet.Cells.GetSubrange("L1", "N1")
                    .Merged = True
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(69, 149, 35), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)

                End With

                With worksheet.Cells("L1")
                    .Value = "TOTALES MES PRODUCCIÓN -- " + MonthName(Date.Today.Month)
                    .Style.Font.Size = 16 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    '.Style.Font.Weight = ExcelFont.BoldWeight
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(69, 149, 35), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                End With
                worksheet.Cells("L2").Value = "Ventas (m" & Chr(178) & ")"
                worksheet.Cells("L4").Value = "Corte (m" & Chr(178) & ")"
                worksheet.Cells("L6").Value = "Producción (m" & Chr(178) & ")"
                worksheet.Cells("L8").Value = "Pendiente (m" & Chr(178) & ")"
                worksheet.Cells("L10").Value = "Reposiciones (m" & Chr(178) & ")"

                worksheet.Cells().GetSubrange("L2", "L3").Merged = True
                worksheet.Cells().GetSubrange("L4", "L5").Merged = True
                worksheet.Cells().GetSubrange("L6", "L7").Merged = True
                worksheet.Cells().GetSubrange("L8", "L9").Merged = True
                worksheet.Cells().GetSubrange("L10", "L11").Merged = True


                worksheet.Cells("M2").Formula = "=SUM(B3:B40)"
                worksheet.Cells("M4").Formula = "=SUM(C3:C40)"
                worksheet.Cells("M6").Formula = "=SUM(D3:D40)"
                worksheet.Cells("M8").Formula = "=SUM(E3:E40)"
                worksheet.Cells("M10").Formula = "=SUM(F3:F40)"

                worksheet.Cells().GetSubrange("M2", "N3").Merged = True
                worksheet.Cells().GetSubrange("M4", "N5").Merged = True
                worksheet.Cells().GetSubrange("M6", "N7").Merged = True
                worksheet.Cells().GetSubrange("M8", "N9").Merged = True
                worksheet.Cells().GetSubrange("M10", "N11").Merged = True

                With worksheet.Cells.GetSubrange("M2", "N11")
                    .Style.Font.Size = 13 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    '.Style.Font.Weight = ExcelFont.BoldWeight
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(92, 92, 92), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                End With

                With worksheet.Cells.GetSubrange("L2", "L11")
                    .Style.Font.Size = 13 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    .Style.Font.Weight = ExcelFont.BoldWeight
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(26, 51, 92), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                End With

                With worksheet.Cells.GetSubrange("L13", "N13")
                    .Merged = True
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(69, 149, 35), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)

                End With

                With worksheet.Cells("L13")
                    .Value = "TOTALES COBROS -- " + MonthName(Date.Today.Month)
                    .Style.Font.Size = 16 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    '.Style.Font.Weight = ExcelFont.BoldWeight
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(69, 149, 35), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                End With


                'xlWorksheet.Range("L14").Formula = "=SUM($G$3:G$34)"
                'xlWorksheet.Range("L14").NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)


                worksheet.Cells("L14").Value = "Efectivo"
                worksheet.Cells("L16").Value = "Cheques"
                worksheet.Cells("L18").Value = "Totales"
                worksheet.Cells().GetSubrange("L14", "L15").Merged = True
                worksheet.Cells().GetSubrange("L16", "L17").Merged = True
                worksheet.Cells().GetSubrange("L18", "L19").Merged = True

                worksheet.Cells("M14").Formula = "=SUM(H3:H40)"
                worksheet.Cells("M14").Style.NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)
                worksheet.Cells("M16").Formula = "=SUM(I3:I40)"
                worksheet.Cells("M16").Style.NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)

                worksheet.Cells("M18").Formula = "=SUM(J3:J40)"
                worksheet.Cells("M18").Style.NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)

                worksheet.Cells().GetSubrange("M14", "N15").Merged = True
                worksheet.Cells().GetSubrange("M16", "N17").Merged = True
                worksheet.Cells().GetSubrange("M18", "N19").Merged = True

                With worksheet.Cells.GetSubrange("M14", "N19")
                    .Style.Font.Size = 13 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    '.Style.Font.Weight = ExcelFont.BoldWeight
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(92, 92, 92), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                End With

                With worksheet.Cells.GetSubrange("L14", "L19")
                    .Style.Font.Size = 13 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    .Style.Font.Weight = ExcelFont.BoldWeight
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(26, 51, 92), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                End With
                '' DIA
                worksheet.Cells("A2").Value = "Fecha"
                worksheet.Cells("B2").Value = "Ventas (m" & Chr(178) & ")"
                worksheet.Cells("C2").Value = "Corte (m" & Chr(178) & ")"
                worksheet.Cells("D2").Value = "Producción (m" & Chr(178) & ")"
                worksheet.Cells("E2").Value = "Pendiente (m" & Chr(178) & ")"
                worksheet.Cells("F2").Value = "Reposiciones (m" & Chr(178) & ")"

                With worksheet.Cells.GetSubrange("A2", "F2")
                    .Style.Font.Size = 13 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(26, 51, 92), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                End With

                worksheet.Cells("H2").Value = "Efectivo"
                worksheet.Cells("I2").Value = "Cheque"
                worksheet.Cells("J2").Value = "Total"


                ' Valores
                Dim daoc As New Consultas
                worksheet.Cells("A3").Value = Date.Today.ToShortDateString
                worksheet.Cells("B3").Value = daoc.getVentasM2()
                worksheet.Cells("C3").Value = daoc.getCorte()
                worksheet.Cells("D3").Value = daoc.getProduccion()
                worksheet.Cells("E3").Value = daoc.getPendientes()
                worksheet.Cells("F3").Value = daoc.getReposicionesDia()

                With worksheet.Cells.GetSubrange("A3", "F3")
                    .Style.Font.Size = 11 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(92, 92, 92), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                End With

                worksheet.Cells("H3").Value = daoc.getEfectivoDia()
                worksheet.Cells("H3").Style.NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)
                worksheet.Cells("I3").Value = daoc.getChequesDia()
                worksheet.Cells("I3").Style.NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)
                worksheet.Cells("J3").Formula = "=SUM(H3:I3)"
                worksheet.Cells("J3").Calculate()
                worksheet.Cells("J3").Style.NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)

                With worksheet.Cells.GetSubrange("H3", "J3")
                    .Style.Font.Size = 11 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(92, 92, 92), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)
                    .Style.WrapText = True
                    .Style.ShrinkToFit = False
                End With

                With worksheet.Cells.GetSubrange("H2", "J2")
                    .Style.Font.Size = 13 * 20
                    .Style.HorizontalAlignment = HorizontalAlignmentStyle.Center
                    .Style.VerticalAlignment = VerticalAlignmentStyle.Center
                    .Style.Font.Color = Color.White
                    .Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.FromArgb(26, 51, 92), Color.Empty)
                    .Style.Borders.SetBorders(MultipleBorders.All, Color.White, LineStyle.Medium)

                End With


                Dim columnCount = worksheet.CalculateMaxUsedColumns()
                For i As Integer = 0 To columnCount - 1
                    worksheet.Columns(i).AutoFit(1)
                Next
                worksheet.Columns(9).AutoFit(1, worksheet.Rows(1), worksheet.Rows(2))
                worksheet.Columns(11).Width = 35 * 256
                'worksheet.Columns(7).Width = 35 * 256
                worksheet.Columns(6).Width = 2 * 256
                worksheet.Columns(10).Width = 2 * 256



                workbook.Save(pathImg)
                ''------------------------------

                ''xlApp = New Excel.Application
                ''If xlApp Is Nothing Then
                ''    Console.Write("Excel is not properly installed!!")
                ''    Return
                ''Else
                ''    Console.Write("Excel is properly installed!!")
                ''End If
                'Dim xlApp As New Excel.Application
                'Dim xlWorkbook As Excel.Workbook = xlApp.Workbooks.Add()
                'Dim xlWorksheet As Excel.Worksheet = CType(xlWorkbook.Sheets(1), Excel.Worksheet)
                'Dim range = xlWorksheet.Range("A2:H3")

                'With range.Borders
                '    .LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Color = Color.White

                'End With
                '' range.Borders(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous
                ''range.BorderAround(LineStyleType.Medium, Color.Blue)
                'xlWorksheet.Rows(1).RowHeight = 40
                'xlWorksheet.Rows(2).RowHeight = 25
                ''' TITULOS
                'xlWorksheet.Cells(1, 1) = "MES: " + MonthName(Date.Today.Month) + " Año: " + Date.Today.Year.ToString
                'xlWorksheet.Cells(1, 7) = "COBROS"

                'xlWorksheet.Cells(2, 1) = "Fecha:"
                'xlWorksheet.Cells(3, 1) = Date.Today
                'Dim daoc As New Consultas
                '' xlWorksheet.Cells(1, 1).interior.color = Color.Green
                'xlWorksheet.Cells(2, 2) = "Venta (m" & Chr(178) & ")"
                'xlWorksheet.Cells(3, 2) = daoc.getVentasM2()

                ''xlWorksheet.Cells(1, 2).interior.color = Color.Green

                'xlWorksheet.Cells(2, 3) = "Corte (m" & Chr(178) & ")"
                'xlWorksheet.Cells(3, 3) = daoc.getCorte()
                ''xlWorksheet.Cells(1, 3).interior.color = Color.Green

                'xlWorksheet.Cells(2, 4) = "Producción (m" & Chr(178) & ")"
                'xlWorksheet.Cells(3, 4) = daoc.getProduccion()
                '' xlWorksheet.Cells(1, 4).interior.color = Color.Green

                'xlWorksheet.Cells(2, 5) = "Pendiente (m" & Chr(178) & ")"
                'xlWorksheet.Cells(3, 5) = daoc.getPendientes()

                'xlWorksheet.Cells(2, 6) = "Reposiciones (m" & Chr(178) & ")"
                'xlWorksheet.Cells(3, 6) = daoc.getReposicionesDia()

                '' xlWorksheet.Cells(1, 5).interior.color = Color.Green
                'xlWorksheet.Cells(2, 7) = "Efectivo"
                'xlWorksheet.Cells(3, 7) = daoc.getEfectivoDia()
                'xlWorksheet.Cells(2, 8) = "Cheques"
                'xlWorksheet.Cells(3, 8) = daoc.getChequesDia()
                'xlWorksheet.Cells(2, 9) = "Totales (Efectivo + Cheques)"


                'With xlWorksheet.Range("A2", "I2")
                '    .Font.Bold = True
                '    .Font.Size = 12
                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(26, 51, 92)
                '    .Font.Color = Color.White
                '    .EntireColumn.AutoFit()
                'End With

                'With xlWorksheet.Range("A3", "I3")
                '    .Font.Bold = False
                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(92, 92, 92)
                '    .Font.Color = Color.White
                '    .EntireColumn.AutoFit()
                'End With

                'With xlWorksheet.Range("A1", "F1")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(69, 149, 35)
                '    .Font.Color = Color.White
                '    .Font.Size = 13


                '    .Borders.Color = Color.White
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Weight = Excel.XlBorderWeight.xlThick
                '    .MergeCells = True
                'End With

                'With xlWorksheet.Range("G1", "I1")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(69, 149, 35)
                '    .Font.Color = Color.White
                '    .Font.Size = 13
                '    .Borders.Color = Color.White
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Weight = Excel.XlBorderWeight.xlThick
                '    .MergeCells = True
                'End With


                ''' TOTALES
                'xlWorksheet.Cells(1, 11) = "TOTALES MES PRODUCCIÓN " + MonthName(Date.Today.Month)
                'xlWorksheet.Cells(13, 11) = "TOTALES MES - COBROS: " + MonthName(Date.Today.Month)
                'xlWorksheet.Cells(14, 11) = "Efectivo: "
                'With xlWorksheet.Range("K14", "K15")
                '    .MergeCells = True
                'End With

                'xlWorksheet.Cells(16, 11) = "Cheques: "
                'With xlWorksheet.Range("K16", "K17")
                '    .MergeCells = True
                'End With

                'xlWorksheet.Cells(18, 11) = "Totales: "
                'With xlWorksheet.Range("K18", "K19")
                '    .MergeCells = True
                'End With

                'xlWorksheet.Cells(2, 11) = "Venta: (m" & Chr(178) & ")"
                'With xlWorksheet.Range("K2", "K3")
                '    .MergeCells = True
                'End With

                'xlWorksheet.Cells(4, 11) = "Corte: (m" & Chr(178) & ")"
                'With xlWorksheet.Range("K4", "K5")
                '    .MergeCells = True
                'End With

                'xlWorksheet.Cells(6, 11) = "Producción: (m" & Chr(178) & ")"
                'With xlWorksheet.Range("K6", "K7")
                '    .MergeCells = True
                'End With

                'xlWorksheet.Cells(8, 11) = "Pendientes: (m" & Chr(178) & ")"
                'With xlWorksheet.Range("K8", "K9")
                '    .MergeCells = True
                'End With

                'xlWorksheet.Cells(10, 11) = "Reposiciones: (m" & Chr(178) & ")"
                'With xlWorksheet.Range("K10", "K11")
                '    .MergeCells = True
                'End With

                'With xlWorksheet.Range("K2", "K11")
                '    .Font.Bold = True
                '    .Font.Size = 12
                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(26, 51, 92)
                '    .Font.Color = Color.White
                '    .EntireColumn.AutoFit()
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Color = Color.White
                'End With

                'With xlWorksheet.Range("K14", "K19")
                '    .Font.Bold = True
                '    .Font.Size = 12
                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(26, 51, 92)
                '    .Font.Color = Color.White
                '    .EntireColumn.AutoFit()
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Color = Color.White
                'End With

                'With xlWorksheet.Range("K1", "M1")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(69, 149, 35)
                '    .Font.Color = Color.White
                '    .Font.Size = 13
                '    .MergeCells = True
                'End With

                'With xlWorksheet.Range("K13", "M13")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(69, 149, 35)
                '    .Font.Color = Color.White
                '    .Font.Size = 13
                '    .MergeCells = True
                'End With

                'xlWorksheet.Range("L2").Formula = "=SUM($B$3:B$34)"
                'xlWorksheet.Range("L2").Formula = "=SUM($B$3:B$34)"
                'xlWorksheet.Range("L4").Formula = "=SUM($C$3:C$34)"
                'xlWorksheet.Range("L6").Formula = "=SUM($D$3:D$34)"
                'xlWorksheet.Range("L8").Formula = "=SUM($E$3:E$34)"
                'xlWorksheet.Range("L10").Formula = "=SUM($F$3:F$34)"

                'Dim strCurrency = Chr(34) & "₲" & Chr(34)
                'xlWorksheet.Range("L14").Formula = "=SUM($G$3:G$34)"
                'xlWorksheet.Range("L14").NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)

                'xlWorksheet.Range("L16").Formula = "=SUM($H$3:H$34)"
                'xlWorksheet.Range("L16").NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)

                'xlWorksheet.Range("L18").Formula = "=SUM($I$3:I$34)"
                'xlWorksheet.Range("L18").NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)

                'xlWorksheet.Range("I3").Formula = "=SUM($G$3:H$3)"
                'xlWorksheet.Range("I3").NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)
                'xlWorksheet.Range("H3").NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)
                'xlWorksheet.Range("G3").NumberFormat = String.Format("{0}###,##0_0;({0}#,##0)", strCurrency)

                'With xlWorksheet.Range("L2", "M3")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(92, 92, 92)
                '    .Font.Color = Color.White
                '    .Font.Size = 13
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Color = Color.White
                '    .MergeCells = True
                'End With

                'With xlWorksheet.Range("L4", "M5")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(92, 92, 92)
                '    .Font.Color = Color.White
                '    .Font.Size = 13
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Color = Color.White
                '    .MergeCells = True
                'End With

                'With xlWorksheet.Range("L6", "M7")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(92, 92, 92)
                '    .Font.Color = Color.White
                '    .Font.Size = 13
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Color = Color.White
                '    .MergeCells = True
                'End With

                'With xlWorksheet.Range("L8", "M9")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(92, 92, 92)
                '    .Font.Color = Color.White
                '    .Font.Size = 13
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Color = Color.White
                '    .MergeCells = True
                'End With

                'With xlWorksheet.Range("L10", "M11")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(92, 92, 92)
                '    .Font.Color = Color.White
                '    .Font.Size = 13
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Color = Color.White
                '    .MergeCells = True
                'End With

                'With xlWorksheet.Range("L14", "M15")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(92, 92, 92)
                '    .Font.Color = Color.White
                '    .Font.Size = 13
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Color = Color.White
                '    .MergeCells = True
                'End With

                'With xlWorksheet.Range("L16", "M17")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(92, 92, 92)
                '    .Font.Color = Color.White
                '    .Font.Size = 13
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Color = Color.White
                '    .MergeCells = True
                'End With

                'With xlWorksheet.Range("L18", "M19")

                '    .VerticalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
                '    .Interior.Color = Color.FromArgb(92, 92, 92)
                '    .Font.Color = Color.White
                '    .Font.Size = 13
                '    .Borders.LineStyle = Excel.XlLineStyle.xlContinuous
                '    .Borders.Color = Color.White
                '    .MergeCells = True
                'End With
                ''Dim oXLRange As Excel.Range
                ''oXLRange = CType(xlWorksheet.Cells(2, 8), Excel.Range)
                ''oXLRange.Formula = "SUM(B3:B3)"

                '''------------------
                'xlWorksheet.SaveAs(pathImg)

                'xlWorkbook.Close()
                'xlApp.Quit()

                'releaseObject(xlApp)
                'releaseObject(xlWorkbook)
                'releaseObject(xlWorksheet)
                'releaseObject(range)
                'range = Nothing
                'xlApp = Nothing
                'xlWorksheet = Nothing
                'xlWorkbook = Nothing
                ''xlApp.Quit()
            End If



            ' xlApp = New Microsoft.Office.Interop.Excel.Application()

            'releaseObject(xlApp)
            mandarMail()
            Console.Write("INFORME ENVIADO")
            Console.Read()


            Threading.Thread.Sleep(1000)
        Catch ex As Exception
            Console.Write(ex.Message)
            Console.Read()
        End Try
    End Sub
    Private Sub mandarMail()
        Try
            Dim Smtp_Server As New SmtpClient
            Dim e_mail As New MailMessage()
            Smtp_Server.UseDefaultCredentials = False
            Smtp_Server.Credentials = New Net.NetworkCredential("sistema.casa.marco@gmail.com", "c@samarco--2019")
            Smtp_Server.Port = 587
            Smtp_Server.EnableSsl = True
            Smtp_Server.Host = "smtp.gmail.com"

            e_mail = New MailMessage()
            e_mail.From = New MailAddress("sistema.casa.marco@gmail.com")
            e_mail.To.Add("marcelo.cardus@gmail.com")
            e_mail.CC.Add("marcelo.cardus@gmail.com")

            'e_mail.CC.Add("samuel.sebastian.ruiz@gmail.com")
            e_mail.Subject = "Oviedo Prueba"
            e_mail.IsBodyHtml = False
            e_mail.Body = "Informe dia " + Date.Today.ToShortDateString
            Dim attachment = New System.Net.Mail.Attachment(pathFile) 'file pat


            e_mail.Attachments.Add(attachment)
            Smtp_Server.Send(e_mail)

            'MsgBox("Mail Sent")

        Catch error_t As Exception
            MsgBox(error_t.ToString)
        End Try
    End Sub
    Private Sub releaseObject(ByVal obj As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            obj = Nothing
        Catch ex As Exception
            obj = Nothing
        Finally
            GC.Collect()
        End Try
    End Sub
End Module

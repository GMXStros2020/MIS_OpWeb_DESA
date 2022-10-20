Imports Microsoft.VisualBasic
Imports Microsoft.Reporting.WebForms
Imports PdfSharp.Pdf
Imports PdfSharp.Pdf.IO
Imports System.Data
Imports System.IO
Imports System.Diagnostics


Public Class reportePDF
    'Dim PathReport As String
    Dim _nro_ops As String()
    Dim _rutaArchivo_correo As String
    Dim _cod_usuario As String
    Dim _ruta_explorador As String

    Public Property Nro_ops As String()
        Get
            Return _nro_ops
        End Get
        Set(value As String())
            _nro_ops = value
        End Set
    End Property

    Public Property RutaArchivo_correo As String
        Get
            Return _rutaArchivo_correo
        End Get
        Set(value As String)
            _rutaArchivo_correo = value
        End Set
    End Property

    Public Property Cod_usuario As String
        Get
            Return _cod_usuario
        End Get
        Set(value As String)
            _cod_usuario = value
        End Set
    End Property

    Public Property Ruta_explorador As String
        Get
            Return _ruta_explorador
        End Get
        Set(value As String)
            _ruta_explorador = value
        End Set
    End Property
    'Dim NombreArchivo As String

    Public Sub GenerarRenombrar()
        Dim archivos() As String = Nothing
        Dim warnings As Warning() = Nothing
        Dim streamids As String() = Nothing
        Dim mimeType As String = Nothing
        Dim encoding As String = Nothing
        Dim extension As String = Nothing
        Dim format As String
        Dim deviceInfo As String
        Dim bytes As Byte()
        Dim dt As New DataTable
        Dim rutaserver As String
        Dim UrlReport As String
        Dim PathReport As String
        Dim UsuaReport As String
        Dim PwdReport As String
        Dim DomReport As String
        Dim rutacompleta As String
        Dim ReportV = New ReportViewer()
        Dim RutaArchivo As String
        Dim rutasalida As String


        Try
            Funciones.fn_Consulta("usp_obtDatosReporteOP 1", dt)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                UrlReport = dt.Rows(0)("url").ToString
                PathReport = "/" + dt.Rows(0)("NombreCarpeta").ToString + "/OrdenPago_stro"
                UsuaReport = dt.Rows(0)("UsuaReport").ToString
                PwdReport = dt.Rows(0)("PwdReport").ToString
                DomReport = dt.Rows(0)("DomReport").ToString
                rutaserver = dt.Rows(0)("rutaserver").ToString
            End If

            ReportV.ProcessingMode = ProcessingMode.Remote
            ReportV.ServerReport.ReportServerUrl = New Uri(UrlReport)
            ReportV.ServerReport.ReportPath = PathReport
            Dim crc As New Custom_ReportCredentials(UsuaReport, PwdReport, DomReport)
            ReportV.ServerReport.ReportServerCredentials = crc
            ReportV.ServerReport.Refresh()

            format = "PDF"
            deviceInfo = "True"

            ReDim archivos(Nro_ops.Length - 1)

            For index = 0 To Nro_ops.Length - 1
                ReportV.ServerReport.SetParameters(New ReportParameter("nro_op", Nro_ops(index)))
                bytes = ReportV.ServerReport.Render(format, Nothing, mimeType, encoding, extension, streamids, warnings)

                rutacompleta = rutaserver + "\\" + Nro_ops(index).ToString() + "-0-.pdf"

                Dim fs As New System.IO.FileStream(rutacompleta, System.IO.FileMode.Create)
                fs.Write(bytes, 0, bytes.Length)
                fs.Close()
                fs.Dispose()

                bytes = Nothing
                archivos(index) = rutacompleta

                rutasalida = rutacompleta.Replace("\\", "\")
                'RutaArchivo_correo = "file:///" + rutasalida
                RutaArchivo_correo = rutasalida
                'System.Diagnostics.Process.Start("explorer", RutaArchivo_correo)

                'Funciones.EjecutaFuncion("fn_abrir_documento('" & rutacompleta & "');")
                'Process.Start(RutaArchivo_correo)
                'System.Diagnostics.Process.Start(RutaArchivo_correo)

                'System.Diagnostics.Process.Start("explorer", "Impresion/419102-0-.pdf")
                ' Dim Drives() As String = System.IO.Directory.GetLogicalDrives()
                'Process.Start("https://www.google.com.mx/?hl=es-419")
                'System.Diagnostics.Process.Start("explorer", "https://www.google.com.mx/?hl=es-419")

                'System.Diagnostics.Process.Start("explorer", "http://localhost:81/419102-0-.pdf")

                'Shell("explorer.exe root=C:\WINDOWS\system32\notepad.exe", vbNormalFocus)
                'Shell("explorer.exe root=C:\Impresion", vbNormalFocus)
                'Shell("explorer.exe root=C:\Impresion\419102-0-.pdf", vbNormalFocus)
                'Shell("explorer.exe root=http://localhost:81/419102-0-.pdf", vbNormalFocus)
                'Mensaje.MuestraMensaje("Error", RutaArchivo_correo, Mensaje.TipoMsg.Advertencia)
            Next




        Catch ex As Exception
            EventLog.CreateEventSource(ex.Message, "Application")
            Mensaje.MuestraMensaje("Error", ex.Message, Mensaje.TipoMsg.Advertencia)
        End Try
    End Sub

    Public Function GenerarRenombrarlocal() As String()
        Dim archivos() As String = Nothing
        Dim warnings As Warning() = Nothing
        Dim streamids As String() = Nothing
        Dim mimeType As String = Nothing
        Dim encoding As String = Nothing
        Dim extension As String = Nothing
        Dim format As String
        Dim deviceInfo As String
        Dim bytes As Byte()
        Dim dt As New DataTable
        Dim rutaserver As String
        Dim UrlReport As String
        Dim PathReport As String
        Dim UsuaReport As String
        Dim PwdReport As String
        Dim DomReport As String
        Dim rutacompleta As String
        Dim ReportV = New ReportViewer()
        Dim RutaArchivo As String
        Dim rutasalida As String
        Dim Arreglorutalocal() As String = Nothing
        Dim Num_op As String


        Try
            Funciones.fn_Consulta("usp_obtDatosReporteOP 1", dt)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                UrlReport = dt.Rows(0)("url").ToString
                PathReport = "/" + dt.Rows(0)("NombreCarpeta").ToString + "/OrdenPago_stro"
                UsuaReport = dt.Rows(0)("UsuaReport").ToString
                PwdReport = dt.Rows(0)("PwdReport").ToString
                DomReport = dt.Rows(0)("DomReport").ToString
                rutaserver = dt.Rows(0)("rutaserverLocal").ToString
            End If

            ReportV.ProcessingMode = ProcessingMode.Remote
            ReportV.ServerReport.ReportServerUrl = New Uri(UrlReport)
            ReportV.ServerReport.ReportPath = PathReport
            Dim crc As New Custom_ReportCredentials(UsuaReport, PwdReport, DomReport)
            ReportV.ServerReport.ReportServerCredentials = crc
            ReportV.ServerReport.Refresh()

            format = "PDF"
            deviceInfo = "True"

            ReDim archivos(Nro_ops.Length - 1)
            ReDim Arreglorutalocal(Nro_ops.Length - 1)

            For index = 0 To Nro_ops.Length - 1
                Num_op = Nro_ops(index).ToString() + "-0-.pdf"
                'Arreglorutalocal(Num_op)
                'Arreglorutalocal = {Num_op}
                Arreglorutalocal(index) = Num_op
                ReportV.ServerReport.SetParameters(New ReportParameter("nro_op", Nro_ops(index)))
                bytes = ReportV.ServerReport.Render(format, Nothing, mimeType, encoding, extension, streamids, warnings)

                'rutacompleta = rutaserver + "//" + Nro_ops(index).ToString() + "-0-.pdf"
                rutacompleta = rutaserver + "//" + Num_op

                Dim fs As New System.IO.FileStream(rutacompleta, System.IO.FileMode.Create)
                fs.Write(bytes, 0, bytes.Length)
                fs.Close()
                fs.Dispose()
                bytes = Nothing



            Next
            Return Arreglorutalocal
        Catch ex As Exception
            EventLog.CreateEventSource(ex.Message, "Application")
            Mensaje.MuestraMensaje("Error", ex.Message, Mensaje.TipoMsg.Advertencia)
        End Try
    End Function


    Public Sub Imprimir_todos()
        Dim archivos() As String = Nothing
        Dim warnings As Warning() = Nothing
        Dim streamids As String() = Nothing
        Dim mimeType As String = Nothing
        Dim encoding As String = Nothing
        Dim extension As String = Nothing
        Dim format As String
        Dim deviceInfo As String
        Dim bytes As Byte()
        Dim dt As New DataTable
        Dim rutaserver As String
        Dim UrlReport As String
        Dim PathReport As String
        Dim UsuaReport As String
        Dim PwdReport As String
        Dim DomReport As String
        Dim rutacompleta As String
        Dim ReportV = New ReportViewer()

        Try
            Funciones.fn_Consulta("usp_obtDatosReporteOP 1", dt)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                UrlReport = dt.Rows(0)("url").ToString
                PathReport = "/" + dt.Rows(0)("NombreCarpeta").ToString + "/OrdenPago_stro"
                UsuaReport = dt.Rows(0)("UsuaReport").ToString
                PwdReport = dt.Rows(0)("PwdReport").ToString
                DomReport = dt.Rows(0)("DomReport").ToString
                rutaserver = dt.Rows(0)("rutaserver").ToString
            End If

            ReportV.ProcessingMode = ProcessingMode.Remote
            ReportV.ServerReport.ReportServerUrl = New Uri(UrlReport)
            ReportV.ServerReport.ReportPath = PathReport
            Dim crc As New Custom_ReportCredentials(UsuaReport, PwdReport, DomReport)
            ReportV.ServerReport.ReportServerCredentials = crc
            ReportV.ServerReport.Refresh()

            format = "PDF"
            deviceInfo = "True"

            ReDim archivos(Nro_ops.Length - 1)

            For index = 0 To Nro_ops.Length - 1
                ReportV.ServerReport.SetParameters(New ReportParameter("nro_op", Nro_ops(index)))
                bytes = ReportV.ServerReport.Render(format, Nothing, mimeType, encoding, extension, streamids, warnings)

                rutacompleta = rutaserver + "\\" + Nro_ops(index).ToString() + "-1-.pdf"

                Dim fs As New System.IO.FileStream(rutacompleta, System.IO.FileMode.Create)
                fs.Write(bytes, 0, bytes.Length)
                fs.Close()
                fs.Dispose()
                bytes = Nothing
                archivos(index) = rutacompleta
            Next

            If Nro_ops.Length > 1 Then
                unirPDFS(archivos, rutaserver)
            End If

        Catch ex As Exception
            'Mensaje.MuestraMensaje("", ex.Message, Mensaje.TipoMsg.Advertencia)
        End Try
    End Sub
    Public Function Imprimir_todos_local() As String
        Dim archivos() As String = Nothing
        Dim warnings As Warning() = Nothing
        Dim streamids As String() = Nothing
        Dim mimeType As String = Nothing
        Dim encoding As String = Nothing
        Dim extension As String = Nothing
        Dim format As String
        Dim deviceInfo As String
        Dim bytes As Byte()
        Dim dt As New DataTable
        Dim rutaserver As String
        Dim UrlReport As String
        Dim PathReport As String
        Dim UsuaReport As String
        Dim PwdReport As String
        Dim DomReport As String
        Dim rutacompleta As String
        Dim ReportV = New ReportViewer()
        Dim RutaArchivo_Local As String

        Try
            Funciones.fn_Consulta("usp_obtDatosReporteOP 1", dt)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                UrlReport = dt.Rows(0)("url").ToString
                PathReport = "/" + dt.Rows(0)("NombreCarpeta").ToString + "/OrdenPago_stro"
                UsuaReport = dt.Rows(0)("UsuaReport").ToString
                PwdReport = dt.Rows(0)("PwdReport").ToString
                DomReport = dt.Rows(0)("DomReport").ToString
                rutaserver = dt.Rows(0)("rutaserverLocal").ToString
            End If

            ReportV.ProcessingMode = ProcessingMode.Remote
            ReportV.ServerReport.ReportServerUrl = New Uri(UrlReport)
            ReportV.ServerReport.ReportPath = PathReport
            Dim crc As New Custom_ReportCredentials(UsuaReport, PwdReport, DomReport)
            ReportV.ServerReport.ReportServerCredentials = crc
            ReportV.ServerReport.Refresh()

            format = "PDF"
            deviceInfo = "True"

            ReDim archivos(Nro_ops.Length - 1)

            For index = 0 To Nro_ops.Length - 1

                ReportV.ServerReport.SetParameters(New ReportParameter("nro_op", Nro_ops(index)))
                bytes = ReportV.ServerReport.Render(format, Nothing, mimeType, encoding, extension, streamids, warnings)

                rutacompleta = rutaserver + "\\" + Nro_ops(index).ToString() + "-1-.pdf"

                Dim fs As New System.IO.FileStream(rutacompleta, System.IO.FileMode.Create)
                fs.Write(bytes, 0, bytes.Length)
                fs.Close()
                fs.Dispose()
                bytes = Nothing
                archivos(index) = rutacompleta
            Next


            If Nro_ops.Length > 1 Then
                RutaArchivo_Local = unirPDFS(archivos, rutaserver)
            End If

            Return RutaArchivo_Local
        Catch ex As Exception
            'Mensaje.MuestraMensaje("", ex.Message, Mensaje.TipoMsg.Advertencia)
        End Try
    End Function
    Private Function unirPDFS(archivos As String(), RutaArchivo As String) As String
        Dim NuevoArchivo As String
        Dim RutaCompleta As String
        Dim RutaArchivo_correo As String
        ' //// Obtener algunos nombres de archivo
        'Dim files As String()
        'files = {]

        '//// Abra el documento de salida
        Dim outputDocument As New PdfDocument()

        '//// recorrer archivos
        For Each archivo As String In archivos
            '// Abrir el documento para importar páginas
            Dim inputDocument As PdfDocument = PdfReader.Open(archivo, PdfDocumentOpenMode.Import)

            '// Recorrer las páginas
            Dim count As Integer = inputDocument.PageCount
            For idx = 0 To count - 1
                '    // Obtener la página del documento externo
                Dim Page As PdfPage = inputDocument.Pages(idx)
                '    // Y agregarlo al documento de salida.
                outputDocument.AddPage(Page)
            Next
        Next

        NuevoArchivo = "Ordenes de pago_stros"
        RutaCompleta = RutaArchivo + "\\" + NuevoArchivo + ".pdf"
        outputDocument.Save(RutaCompleta)

        For Each archivo As String In archivos
            System.IO.File.Delete(archivo.ToString())
        Next

        '...Abrir el archivo
        RutaCompleta.Replace("\\", "\")

        RutaArchivo_correo = "Ordenes de pago_stros.pdf"
        Return RutaArchivo_correo

        'Process.Start(RutaArchivo_correo)
        'System.Diagnostics.Process.Start(RutaArchivo_correo)
        'System.Diagnostics.Process.Start("explorer", RutaArchivo_correo)

    End Function

    Public Function inicializaArbolfolder(accion As Integer, sPathFile As String, nro_op As String) As String

        Dim fecha As Date
        Dim sAnio As String
        Dim iMes As Integer
        Dim sMes As String
        Dim sDia As String
        Dim dt As New DataTable


        If accion = 1 Then
            sAnio = DateTime.Now.Year.ToString()
        Else
            Funciones.fn_Consulta("usp_obtDatosReporteOP 2, " & nro_op, dt)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                fecha = dt.Rows(0)("fec_generacion")
            End If

            sAnio = fecha.Year.ToString()
            iMes = fecha.Month
            sMes = fecha.Month.ToString()
            sDia = fecha.Day.ToString()

            If iMes = 1 Then sMes = "ENE" ' "Enero"
            If iMes = 2 Then sMes = "FEB" ' "Febrero"
            If iMes = 3 Then sMes = "MAR" ' "Marzo"
            If iMes = 4 Then sMes = "ABR" ' "Abril"
            If iMes = 5 Then sMes = "MAY" ' "Mayo"
            If iMes = 6 Then sMes = "JUN" ' "Junio"
            If iMes = 7 Then sMes = "JUL" ' "Julio"
            If iMes = 8 Then sMes = "AGO" ' "Agosto"
            If iMes = 9 Then sMes = "SEP" ' "Septiembre"
            If iMes = 10 Then sMes = "OCT" ' "Octubre"
            If iMes = 11 Then sMes = "NOV" ' "Noviembre"
            If iMes = 12 Then sMes = "DIC" ' "Diciembre"
        End If

        'AÑO
        If Directory.Exists(sPathFile + "\\" + sAnio) Then
            sPathFile = sPathFile + "\\" + sAnio
        Else
            Directory.CreateDirectory(sPathFile + "\\" + sAnio)
            sPathFile = sPathFile + "\\" + sAnio
        End If

        If accion = 1 Then
            'Crea carpeta temporal
            Directory.CreateDirectory(sPathFile + "\\tmp") 'CAMBIAR NOMBRE DE LA CARPETA EN LA QUE SE GUARDARAN LOS PDFS CON VARIAS OPS
            sPathFile = sPathFile + "\\tmp"
        Else
            'Mes
            If Directory.Exists(sPathFile + "\\" + sMes) Then

                sPathFile = sPathFile + "\\" + sMes
            Else
                Directory.CreateDirectory(sPathFile + "\\" + sMes)
                sPathFile = sPathFile + "\\" + sMes
            End If

            'Dia
            If Directory.Exists(sPathFile + "\\" + sDia) Then

                sPathFile = sPathFile + "\\" + sDia
            Else
                Directory.CreateDirectory(sPathFile + "\\" + sDia)
                sPathFile = sPathFile + "\\" + sDia
            End If
        End If

        Return sPathFile
    End Function

    Public Sub ReportePagosInter(sn_impresion As Boolean)
        Dim archivos() As String = Nothing
        Dim warnings As Warning() = Nothing
        Dim streamids As String() = Nothing
        Dim mimeType As String = Nothing
        Dim encoding As String = Nothing
        Dim extension As String = Nothing
        Dim format As String
        Dim deviceInfo As String
        Dim bytes As Byte()
        Dim dt As New DataTable
        Dim rutaserver As String
        Dim UrlReport As String
        Dim PathReport As String
        Dim UsuaReport As String
        Dim PwdReport As String
        Dim DomReport As String
        Dim rutacompleta As String
        Dim ReportV = New ReportViewer()
        Dim RutaArchivo As String

        Try
            Funciones.fn_Consulta("usp_obtDatosReporteOP 3", dt)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                UrlReport = dt.Rows(0)("url").ToString
                PathReport = "/" + dt.Rows(0)("NombreCarpeta").ToString + "/OP_PagoInternacional "
                UsuaReport = dt.Rows(0)("UsuaReport").ToString
                PwdReport = dt.Rows(0)("PwdReport").ToString
                DomReport = dt.Rows(0)("DomReport").ToString
                rutaserver = dt.Rows(0)("rutaserver").ToString
            End If

            ReportV.ProcessingMode = ProcessingMode.Remote
            ReportV.ServerReport.ReportServerUrl = New Uri(UrlReport)
            ReportV.ServerReport.ReportPath = PathReport
            Dim crc As New Custom_ReportCredentials(UsuaReport, PwdReport, DomReport)
            ReportV.ServerReport.ReportServerCredentials = crc
            ReportV.ServerReport.Refresh()

            format = "PDF"
            deviceInfo = "True"

            ReDim archivos(Nro_ops.Length - 1)

            'If Nro_ops.Length > 1 Then
            RutaArchivo = inicializaArbolfolderPI(rutaserver)
            'Else
            'RutaArchivo = inicializaArbolfolderPI(2, rutaserver, Nro_ops(0))
            'End If

            For index = 0 To Nro_ops.Length - 1
                ReportV.ServerReport.SetParameters(New ReportParameter("nroOP", Nro_ops(index)))
                bytes = ReportV.ServerReport.Render(format, Nothing, mimeType, encoding, extension, streamids, warnings)

                Dim i As Integer = 0
                Dim sufijo As Integer = 0

                'If Nro_ops.Length > 1 Then
                '    rutacompleta = RutaArchivo + "\\" + Nro_ops(index).ToString() + "_" + Cod_usuario + ".pdf"
                'Else
                '    rutacompleta = RutaArchivo + "\\Ordenes_de_Pago_Pago_Inter" + "_" + Cod_usuario + ".pdf"
                'End If
                If Nro_ops.Length > 1 Then
                    While System.IO.File.Exists(RutaArchivo + "\\Ordenes_de_Pago_Pago_Inter_tmp" + "_" + Cod_usuario + "-" + i.ToString() + "-.pdf")
                        i += 1
                        'rutacompleta = RutaArchivo + "\\" + Nro_ops(index).ToString() + "-" + i.ToString() + "-.pdf"
                        'rutacompleta = RutaArchivo + "\\" + Nro_ops(index).ToString() + "_" + Cod_usuario + i.ToString() + "-.pdf"
                        rutacompleta = RutaArchivo + "\\Ordenes_de_Pago_Pago_Inter_tmp" + "_" + Cod_usuario + "-" + i.ToString() + "-.pdf"
                    End While
                    If i = 0 Then
                        ' rutacompleta = RutaArchivo + "\\" + Nro_ops(index).ToString() + "-0-.pdf"
                        rutacompleta = RutaArchivo + "\\Ordenes_de_Pago_Pago_Inter_tmp" + "_" + Cod_usuario + "-0-.pdf"
                    End If
                Else
                    While System.IO.File.Exists(RutaArchivo + "\\Ordenes_de_Pago_Pago_Inter" + "_" + Cod_usuario + "-" + i.ToString() + "-.pdf")
                        i += 1
                        rutacompleta = RutaArchivo + "\\Ordenes_de_Pago_Pago_Inter" + "_" + Cod_usuario + "-" + i.ToString() + "-.pdf"
                    End While
                    If i = 0 Then
                        rutacompleta = RutaArchivo + "\\Ordenes_de_Pago_Pago_Inter" + "_" + Cod_usuario + "-0-.pdf"
                    End If
                End If

                Dim fs As New System.IO.FileStream(rutacompleta, System.IO.FileMode.Create)
                fs.Write(bytes, 0, bytes.Length)
                fs.Close()
                fs.Dispose()
                bytes = Nothing
                archivos(index) = rutacompleta
            Next

            If Nro_ops.Length > 1 Then
                unirPDFS_PI(archivos, RutaArchivo, sn_impresion)
            Else
                RutaArchivo_correo = rutacompleta
                If sn_impresion Then
                    ' Process.Start(rutacompleta) 'VZAVALETA_10290_CC7_PDF
                End If
            End If
            Mensaje.MuestraMensaje("Envió para pagos internacionales", "El archivo se guardo correctamente en la siguiente ruta: " + "\n " + rutaserver, Mensaje.TipoMsg.Advertencia)

        Catch ex As Exception
            ' Mensaje.MuestraMensaje("", ex.Message, Mensaje.TipoMsg.Advertencia)
        End Try
    End Sub

    Public Function inicializaArbolfolderPI(sPathFile As String) As String

        Dim fecha As Date
        Dim sAnio As String
        Dim sMes As String
        Dim iMes As Integer
        Dim sDia As String
        Dim dt As New DataTable

        sAnio = DateTime.Now.Year.ToString()
        sMes = DateTime.Now.Month.ToString()
        sDia = DateTime.Now.Day.ToString()
        iMes = DateTime.Now.Month

        If iMes = 1 Then sMes = "ENE" ' "Enero"
        If iMes = 2 Then sMes = "FEB" ' "Febrero"
        If iMes = 3 Then sMes = "MAR" ' "Marzo"
        If iMes = 4 Then sMes = "ABR" ' "Abril"
        If iMes = 5 Then sMes = "MAY" ' "Mayo"
        If iMes = 6 Then sMes = "JUN" ' "Junio"
        If iMes = 7 Then sMes = "JUL" ' "Julio"
        If iMes = 8 Then sMes = "AGO" ' "Agosto"
        If iMes = 9 Then sMes = "SEP" ' "Septiembre"
        If iMes = 10 Then sMes = "OCT" ' "Octubre"
        If iMes = 11 Then sMes = "NOV" ' "Noviembre"
        If iMes = 12 Then sMes = "DIC" ' "Diciembre"

        'AÑO
        If Directory.Exists(sPathFile + "\\" + sAnio) Then

            sPathFile = sPathFile + "\\" + sAnio
        Else
            Directory.CreateDirectory(sPathFile + "\\" + sAnio)
            sPathFile = sPathFile + "\\" + sAnio
        End If

        sPathFile = sPathFile + "\\Pago_Internacional"

        'Mes
        If Directory.Exists(sPathFile + "\\" + sMes) Then

            sPathFile = sPathFile + "\\" + sMes
        Else
            Directory.CreateDirectory(sPathFile + "\\" + sMes)
            sPathFile = sPathFile + "\\" + sMes
        End If

        'Dia
        If Directory.Exists(sPathFile + "\\" + sDia) Then

            sPathFile = sPathFile + "\\" + sDia
        Else
            Directory.CreateDirectory(sPathFile + "\\" + sDia)
            sPathFile = sPathFile + "\\" + sDia
        End If

        Ruta_explorador = sPathFile
        Return sPathFile
    End Function

    Private Sub unirPDFS_PI(archivos As String(), RutaArchivo As String, sn_impresion As Boolean)
        Dim NuevoArchivo As String
        Dim RutaCompleta As String
        ' // Obtener algunos nombres de archivo
        'Dim files As String()
        'files = {]

        '// Abra el documento de salida
        Dim outputDocument As New PdfDocument()

        '// recorrer archivos
        For Each archivo As String In archivos
            '// Abrir el documento para importar páginas
            Dim inputDocument As PdfDocument = PdfReader.Open(archivo, PdfDocumentOpenMode.Import)

            '// Recorrer las páginas
            Dim count As Integer = inputDocument.PageCount
            For idx = 0 To count - 1
                '    // Obtener la página del documento externo
                Dim Page As PdfPage = inputDocument.Pages(idx)
                '    // Y agregarlo al documento de salida.
                outputDocument.AddPage(Page)
            Next
        Next

        Dim i As Integer = 0

        While System.IO.File.Exists(RutaArchivo + "\\Ordenes_de_Pago_Pago_Inter" + "_" + Cod_usuario + "-" + i.ToString() + "-.pdf")
            i += 1
            RutaCompleta = RutaArchivo + "\\Ordenes_de_Pago_Pago_Inter" + "_" + Cod_usuario + "-" + i.ToString() + "-.pdf"
        End While
        If i = 0 Then
            RutaCompleta = RutaArchivo + "\\Ordenes_de_Pago_Pago_Inter" + "_" + Cod_usuario + "-0-.pdf"
        End If


        'NuevoArchivo = "Ordenes_de_Pago_Pago_Inter"
        'RutaCompleta = RutaArchivo + "\\" + NuevoArchivo + "_" + Cod_usuario + ".pdf"
        outputDocument.Save(RutaCompleta)

        For Each archivo As String In archivos
            System.IO.File.Delete(archivo.ToString())
        Next

        '...Abrir el archivo
        RutaArchivo_correo = RutaCompleta
        If sn_impresion Then
            'Process.Start(RutaCompleta) 'VZAVALETA_10290_CC7_PDF
        End If

    End Sub

End Class

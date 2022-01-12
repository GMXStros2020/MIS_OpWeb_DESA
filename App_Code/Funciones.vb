Imports System.ComponentModel
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net.Mail
Imports Mensaje
Imports Microsoft.VisualBasic
Imports System.IO.DirectoryInfo
Imports System.Net

Public Class Funciones

    Enum enumFormatoFecha : int
        YYYYMMDD = 1
        MMDDYYYY
        DDMMYYYY
    End Enum

    Public Shared Function IsAuthenticated(ByVal Domain As String, ByVal username As String, ByVal pwd As String) As Boolean
        Dim Success As Boolean = False
        Dim Entry As New System.DirectoryServices.DirectoryEntry("LDAP://" & Domain, username, pwd)
        Dim Searcher As New System.DirectoryServices.DirectorySearcher(Entry)
        Searcher.SearchScope = DirectoryServices.SearchScope.OneLevel
        Try
            Dim Results As System.DirectoryServices.SearchResult = Searcher.FindOne
            Success = Not (Results Is Nothing)
        Catch
            Success = False
        End Try
        Return Success
    End Function

    'Validación de Active Directory
    Public Shared Function AutenticaUsuario(ByVal Usuario As String, ByVal Contraseña As String) As Boolean
        Dim ws As New ws_Generales.GeneralesClient
        Return ws.IsAuthenticated("GMX.COM.MX", Usuario, Contraseña)
    End Function

    Public Shared Function EjecutaFuncion(ByVal funcion As String, Optional ByVal Key As String = "Funcion") As Boolean
        Dim page As Page = HttpContext.Current.CurrentHandler
        ScriptManager.RegisterClientScriptBlock(page, GetType(Page), Key, funcion, True)
        Return True
    End Function


    Public Shared Function AbrirModal(ByVal modal As String) As Boolean
        Dim page As Page = HttpContext.Current.CurrentHandler
        ScriptManager.RegisterClientScriptBlock(page, GetType(Page), "Abrir", "fn_AbrirModal('" & modal & "');", True)
        Return True
    End Function

    Public Shared Function CerrarModal(ByVal modal As String) As Boolean
        Dim page As Page = HttpContext.Current.CurrentHandler
        ScriptManager.RegisterClientScriptBlock(page, GetType(Page), "Cerrar", "fn_CerrarModal('" & modal & "');", True)
        Return True
    End Function

    Public Shared Function Lista_A_Datatable(Of T)(iList As List(Of T)) As DataTable
        Dim dataTable As New DataTable()
        Dim propertyDescriptorCollection As PropertyDescriptorCollection = TypeDescriptor.GetProperties(GetType(T))
        For i As Integer = 0 To propertyDescriptorCollection.Count - 1
            Dim propertyDescriptor As PropertyDescriptor = propertyDescriptorCollection(i)
            Dim type As Type = propertyDescriptor.PropertyType

            If type.IsGenericType AndAlso type.GetGenericTypeDefinition() = GetType(Nullable(Of )) Then
                type = Nullable.GetUnderlyingType(type)
            End If

            dataTable.Columns.Add(propertyDescriptor.Name, type)
        Next
        Dim values As Object() = New Object(propertyDescriptorCollection.Count - 1) {}
        For Each iListItem As T In iList
            For i As Integer = 0 To values.Length - 1
                values(i) = propertyDescriptorCollection(i).GetValue(iListItem)
            Next
            dataTable.Rows.Add(values)
        Next
        Return dataTable
    End Function

    Public Shared Sub LlenaCatDDL(DDL As DropDownList, Prefijo As String, Optional Condicion As String = "", Optional Sel As String = "",
                                  Optional DataValue As String = "Clave", Optional DataText As String = "Descripcion", Optional SelCurrent As Integer = 0,
                                  Optional optTodas As Boolean = True)
        Dim Resultado As New DataTable
        Try
            Dim ws As New ws_Generales.GeneralesClient

            Resultado = Funciones.Lista_A_Datatable(ws.ObtieneCatalogo(Prefijo, Condicion, Sel).ToList)

            'fn_Consulta("spS_CatalogosSIR '" & Prefijo & "','" & Condicion & "','" & Sel & "'", Resultado)

            If Not Resultado Is Nothing Then
                DDL.DataValueField = DataValue
                DDL.DataTextField = DataText
                DDL.DataSource = Resultado
                DDL.DataBind()

                Dim ultimoPts As Integer

                If optTodas = True Then
                    Dim opcion As ListItem
                    opcion = New ListItem(". . .", "-1")
                    DDL.Items.Add(opcion)
                    ultimoPts = DDL.Items.Count
                End If

                If SelCurrent > 0 Then
                    DDL.SelectedValue = SelCurrent
                ElseIf SelCurrent = -1 Then
                    DDL.SelectedIndex = ultimoPts - 1
                End If



            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje("Carga DDL", "Ocurrio un Error llenar DDL", TipoMsg.Falla)
        End Try
    End Sub
    Public Shared Sub LlenaCatGrid(ByRef Grid As GridView, Prefijo As String, Optional Condicion As String = "", Optional Sel As String = "")
        Dim Resultado As IList = Nothing
        Try
            Dim ws As New ws_Generales.GeneralesClient
            Resultado = ws.ObtieneCatalogo(Prefijo, Condicion, Sel).ToList

            ' fn_Consulta("spS_CatalogosSIR '" & Prefijo & "','" & Condicion & "','" & Sel & "'", Resultado)
            If Not Resultado Is Nothing Then
                Grid.DataSource = Resultado
                Grid.DataBind()
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje("Carga Grid", "Ocurrio un Error llenar Grid", TipoMsg.Falla)
        End Try
    End Sub

    Public Shared Sub LlenaDDL(DDL As DropDownList, ByRef dtDatos As DataTable,
                                  Optional DataValue As String = "Clave", Optional DataText As String = "Descripcion", Optional SelCurrent As Integer = 0,
                                  Optional optTodas As Boolean = True)
        Try

            DDL.DataValueField = DataValue
            DDL.DataTextField = DataText
            DDL.DataSource = dtDatos
            DDL.DataBind()

            Dim ultimoPts As Integer

            If optTodas = True Then
                Dim opcion As ListItem
                opcion = New ListItem(". . .", "-1")
                DDL.Items.Add(opcion)
                ultimoPts = DDL.Items.Count
            End If

            'If SelCurrent <> 0 Then
            '    DDL.SelectedValue = SelCurrent
            'End If
            If SelCurrent > 0 Then
                DDL.SelectedValue = SelCurrent
            ElseIf SelCurrent = -1 Then
                DDL.SelectedIndex = ultimoPts - 1
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("Carga DDL", "Ocurrio un Error llenar DDL", TipoMsg.Falla)
        End Try
    End Sub

    Public Shared Sub LlenaGrid(ByRef gvd_Control As GridView, ByRef dtDatos As DataTable)
        gvd_Control.DataSource = dtDatos
        gvd_Control.DataBind()
    End Sub

    Public Shared Function ObtieneElementos(ByRef Gvd As GridView, ByVal Catalogo As String, ByVal blnTexto As Boolean, Optional PolxIdpv As Boolean = False) As String
        Dim strDatos As String = ""
        For Each row As GridViewRow In Gvd.Rows
            Dim Elemento = DirectCast(row.FindControl("chk_Sel" & Catalogo), HiddenField)
            If Elemento.Value <> "true" Then
                If PolxIdpv = True And Catalogo = "Pol" Then
                    strDatos = strDatos & IIf(blnTexto, ",'", ",") & DirectCast(row.FindControl("lbl_idpv"), Label).Text & IIf(blnTexto, "'", "")
                Else
                    strDatos = strDatos & IIf(blnTexto, ",'", ",") & DirectCast(row.FindControl("lbl_Clave" & Catalogo), Label).Text & IIf(blnTexto, "'", "")
                End If

            End If
        Next

        If Len(strDatos) > 0 Then
            strDatos = Mid(strDatos, 2, Len(strDatos) - 1)
        End If

        Return strDatos
    End Function


    Public Shared Function fn_InsertaBitacora(ByVal cod_modulo As Integer, ByVal cod_submodulo As Integer, ByVal cod_usuario As String, ByVal descripcion As String) As Boolean
        'Dim ws As New ws_Generales.GeneralesClient
        'InsertaBitacora = ws.InsertaBitacora(cod_modulo, cod_submodulo, cod_usuario, descripcion)
        Dim Resultado As String
        Dim Comando As SqlClient.SqlCommand
        Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("CadenaConexion").ConnectionString)
        conn.Open()

        Comando = New SqlClient.SqlCommand("spI_Bitacora " & cod_modulo & "," & cod_submodulo & ",'" & cod_usuario & "','" & Mid(Replace(descripcion, "'", "|"), 1, 8000) & "'", conn)
        Resultado = Convert.ToInt32(Comando.ExecuteScalar())

        conn.Close()
        Return True
    End Function

    Public Shared Function fn_InsertaExcepcion(ByVal cod_modulo As Integer, ByVal cod_submodulo As Integer, ByVal cod_usuario As String, ByVal descripcion As String) As Boolean
        Dim Resultado As String
        Dim Comando As SqlClient.SqlCommand
        Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("CadenaConexion").ConnectionString)
        conn.Open()

        Comando = New SqlClient.SqlCommand("spI_ErrorWeb " & cod_modulo & "," & cod_submodulo & ",'" & cod_usuario & "','" & Mid(Replace(descripcion, "'", "|"), 1, 8000) & "'", conn)
        Resultado = Convert.ToInt32(Comando.ExecuteScalar())

        conn.Close()
        Return True
    End Function

    Public Shared Function fn_InsertaTransaccion(ByVal cod_modulo As Integer, ByVal cod_submodulo As Integer, ByVal Tabla As String, ByVal Key As String,
                                                 ByVal Datos As String, ByVal cod_usuario As String,
                                                 Optional ByVal conexion As SqlConnection = Nothing,
                                                 Optional ByVal transaccion As SqlTransaction = Nothing) As Boolean
        Dim Resultado As String
        Dim Comando As SqlClient.SqlCommand
        Dim conn As SqlConnection = conexion

        Comando = New SqlClient.SqlCommand("spI_Transaccion " & cod_modulo & "," & cod_submodulo & ",'" & Tabla & "','" & Replace(Key, "'", "|") & "','" & Mid(Replace(Datos, "'", "|"), 1, 8000) & "','" & cod_usuario & "'", conn)
        Comando.Transaction = transaccion
        Resultado = Convert.ToInt32(Comando.ExecuteScalar())

        Return True
    End Function

    Public Shared Function fn_Consulta(ByVal Consulta As String, ByRef dtResultado As DataTable) As DataTable
        Dim da As SqlDataAdapter
        Dim conexion = ConfigurationManager.ConnectionStrings("CadenaConexion").ConnectionString

        dtResultado = New DataTable
        da = New SqlDataAdapter(Consulta, conexion)
        da.SelectCommand.CommandTimeout = 1800
        da.Fill(dtResultado)

        Return dtResultado
    End Function

    Public Shared Function fn_Ejecuta(ByVal Consulta As String) As Integer
        Dim conexion As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("CadenaConexion").ConnectionString)
        Dim comandoSQL As SqlClient.SqlCommand
        Dim intResultado As Integer = 0

        conexion.Open()

        comandoSQL = New SqlClient.SqlCommand(Consulta, conexion)
        intResultado = Convert.ToInt32(comandoSQL.ExecuteScalar())

        conexion.Close()

        Return intResultado
    End Function
    Public Shared Function fn_Ejecuta(ByVal Consulta As String, Optional ConexionUAT As Boolean = False) As Integer
        Dim conexion As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("CadenaUAT").ConnectionString)
        Dim comandoSQL As SqlClient.SqlCommand
        Dim intResultado As Integer = 0

        conexion.Open()

        comandoSQL = New SqlClient.SqlCommand(Consulta, conexion)
        intResultado = Convert.ToInt32(comandoSQL.ExecuteScalar())

        conexion.Close()

        Return intResultado
    End Function

    Public Shared Function fn_EjecutaStr(ByVal Consulta As String) As String
        Dim conexion As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("CadenaConexion").ConnectionString)
        Dim comandoSQL As SqlClient.SqlCommand
        Dim strResultado As String = ""

        conexion.Open()

        comandoSQL = New SqlClient.SqlCommand(Consulta, conexion)
        strResultado = Convert.ToString(comandoSQL.ExecuteScalar())

        conexion.Close()

        Return strResultado
    End Function

    Public Shared Function fn_ObtieneDatos_DT(ByRef dtDatos As DataTable, ByVal ArrayCampos() As String, Optional ByVal Condicion As String = vbNullString, Optional ByVal blnKey As Boolean = True) As String()
        Dim id_Datos As Integer = 0
        Dim strDatos(0) As String
        Dim TipoDato As String
        Dim Campo As String = vbNullString

        Dim myRow() As Data.DataRow
        myRow = dtDatos.Select(Condicion)

        For Each row In myRow
            If Len(strDatos(id_Datos)) > 7500 Then
                id_Datos = id_Datos + 1
                ReDim Preserve strDatos(id_Datos)
                strDatos(id_Datos) = ""
            End If

            strDatos(id_Datos) = strDatos(id_Datos) & IIf(blnKey = True, "(@strKey,", "(")

            For Each elemento In ArrayCampos
                If InStr(elemento, ":") > 0 Then
                    Campo = Split(elemento, ":")(0)
                    TipoDato = Split(elemento, ":")(1)
                Else
                    Campo = elemento
                    TipoDato = "N"
                End If

                Select Case TipoDato
                    Case "N"
                        If IsNumeric(row(Campo)) Then
                            strDatos(id_Datos) = strDatos(id_Datos) & row(Campo) & ","
                        Else
                            strDatos(id_Datos) = strDatos(id_Datos) & "NULL" & ","
                        End If
                    Case "T"
                        strDatos(id_Datos) = strDatos(id_Datos) & "''" & row(Campo) & "'',"
                    Case "F"
                        If IsDate(row(Campo)) Then
                            strDatos(id_Datos) = strDatos(id_Datos) & "''" & CDate(row(Campo)).ToString("yyyyMMdd") & "'',"
                        Else
                            strDatos(id_Datos) = strDatos(id_Datos) & "NULL" & ","
                        End If
                    Case "B"
                        If IsNumeric(row(Campo)) Then
                            strDatos(id_Datos) = strDatos(id_Datos) & (-1 * row(Campo)) & ","
                        Else
                            strDatos(id_Datos) = strDatos(id_Datos) & "NULL" & ","
                        End If
                    Case "DN"
                        strDatos(id_Datos) = strDatos(id_Datos) & Campo & ","
                    Case "DT"
                        strDatos(id_Datos) = strDatos(id_Datos) & "''" & Campo & "'',"
                End Select
            Next
            strDatos(id_Datos) = Mid(strDatos(id_Datos), 1, Len(strDatos(id_Datos)) - 1)
            strDatos(id_Datos) = strDatos(id_Datos) & "),"
        Next

        fn_ObtieneDatos_DT = strDatos
    End Function

    Public Shared Function fn_FormatoFecha(ByVal Fecha As String) As String
        If Fecha <> vbNullString Then
            If Not IsDate(Fecha) And Len(Fecha) = 10 Then
                Fecha = Mid(Fecha, 4, 2) & "/" & Mid(Fecha, 1, 2) & "/" & Mid(Fecha, 7, 4)
            End If

            Return String.Format("{0:yyyyMMdd}", CDate(Fecha))
        Else
            Return ""
        End If
    End Function

    Public Shared Function fn_TipoCambio(ByVal Fecha As String, ByVal cod_moneda As Integer) As Double
        fn_TipoCambio = fn_EjecutaStr("spS_TipoCambio '" & Fecha & "'," & cod_moneda)
    End Function

    Public Shared Function fn_Parametro(ByVal Parametro As Integer) As String
        Dim ws As New ws_Generales.GeneralesClient
        fn_Parametro = ws.ObtieneParametro(Parametro)
        Return fn_Parametro
    End Function

    Public Shared Sub fn_Imprime_OP(ByVal url_reportes As String, ByVal strOp As String)
        Dim dtPolizas As DataTable = New DataTable
        Dim dtAsientos As DataTable = New DataTable

        Dim i As Integer
        Dim server As String = Replace(Replace(url_reportes, "@Reporte", "OrdenPago"), "@Formato", "PDF")

        'Asiento de Diario
        fn_Consulta("spS_AsientosXOP '" & strOp & "'", dtAsientos)
        i = 0
        For Each row In dtAsientos.Rows
            EjecutaFuncion("fn_Imprime_Reporte('" & Replace(server, "OrdenPago", "AsientoOP") & "&nro_op=" & row("nro_op") & "&nro_asiento=" & row("nro_asiento") & "&nro_recibo=" & row("nro_recibo") & "');", "ImpresionAsiento" & i)
        Next

        'Cobranzas
        fn_Consulta("spS_PolizaCobXOP '" & strOp & "'", dtPolizas)
        i = 0
        For Each row In dtPolizas.Rows
            If row("sn_financiado") = 0 And (row("sn_pago") <> 0 Or row("sn_recargo") <> 0) Then
                EjecutaFuncion("fn_Imprime_Reporte('" & Replace(server, "OrdenPago", "CobranzaOP") & "&str_pol=+" & row("poliza") & "+&nro_endoso=" & row("nro_endoso") & "&nro_op=" & row("nro_op") & "');", "ImpresionCobranza" & i)
            End If
            i = i + 1
        Next

        EjecutaFuncion("fn_Imprime_SoporteOP('" & Replace(server & "&nro_op=@nro_op", "OrdenPago", "SoporteOP") & "','" & strOp & "');", "ImpresionSoporte")
        EjecutaFuncion("fn_Imprime_OP('" & server & "&nro_op=@nro_op" & "','" & strOp & "');", "Impresion")

    End Sub


    Public Shared Function fn_Guarda_OP(ByVal strOp As String, ByVal usuario As String, ByVal Contraseña As String, Optional ByVal blnGeneracion As Boolean = False, Optional ByVal blnDevolucion As Boolean = False) As Boolean
        Try
            Dim dtOrden As DataTable = New DataTable
            Dim stream As FileStream
            Dim results As Byte()
            Dim Repositorio = fn_Parametro(Cons.Repositorio)
            Dim Carpeta As String = vbNullString
            Dim Año, Dia As String
            Dim Mes As String
            Dim fecha_generacion As Date
            Dim rutas As String

            Funciones.fn_Consulta("spS_OrdenPago '" & strOp & "','','','','','','','','',-1,'','','',0,'',''", dtOrden)

            Dim rsExec As ReportExecutionService.ReportExecutionService = New ReportExecutionService.ReportExecutionService()
            rsExec.Credentials = New NetworkCredential(usuario, Contraseña, "GMX.COM.MX") ' System.Net.CredentialCache.DefaultCredentials

            Dim reportOP As String = "/ReportesGMX/OrdenPago"
            rsExec.LoadReport(reportOP, Nothing)
            Dim executionParams As ReportExecutionService.ParameterValue()
            executionParams = New ReportExecutionService.ParameterValue(0) {}
            executionParams(0) = New ReportExecutionService.ParameterValue()
            executionParams(0).Name = "nro_op"

            For Each row In dtOrden.Rows
                fecha_generacion = CDate(row("fec_generacion"))

                Año = Year(fecha_generacion)
                Mes = fn_ObtieneMes(Month(fecha_generacion))
                Dia = String.Format("{0:00}", Day(fecha_generacion))

                executionParams(0).Value = row("nro_op")
                rsExec.SetExecutionParameters(executionParams, "en-us")
                Dim warnings As ReportExecutionService.Warning() = Nothing
                Dim streamIDs As String() = Nothing

                results = rsExec.Render("PDF", Nothing, vbNullString, vbNullString, vbNullString, warnings, Nothing)

                Dim request As WebClient = New WebClient()
                request.Credentials = New NetworkCredential(usuario, Contraseña, "GMX.COM.MX")

                Using request
                    'Verificar la existencia del Directorio
                    Carpeta = Repositorio & Año
                    If Not Directory.Exists(Carpeta) Then
                        Directory.CreateDirectory(Carpeta)
                    End If

                    Carpeta = Carpeta & "\" & Mes
                    If Not Directory.Exists(Carpeta) Then
                        Directory.CreateDirectory(Carpeta)
                    End If

                    Carpeta = Carpeta & "\" & Dia
                    If Not Directory.Exists(Carpeta) Then
                        Directory.CreateDirectory(Carpeta)
                    End If
                    stream = File.OpenWrite(Carpeta & "\" & row("nro_op") & ".PDF")
                    stream.Write(results, 0, results.Length)
                    stream.Close()
                End Using


                If row("cod_estatus_op") <> 6 Then

                    'Si se genera por primera vez
                    If blnGeneracion = True Then
                        'Soporte de Reaseguro de la OP
                        fn_Guarda_SoporteOP(row("nro_op"), Carpeta, usuario, Contraseña)

                        'Si no se trata de una devolución guarda el soporte de cobranza
                        If blnDevolucion = False Then
                            'Soporte de Cobranza de la OP
                            fn_Guarda_CobranzaOP(row("nro_op"), Carpeta, usuario, Contraseña)
                        End If

                        'Asiento de Diario
                        fn_Guarda_AsientoOP(row("nro_op"), Carpeta, usuario, Contraseña)
                    End If


                    'Guarda Notificacion
                    fn_Guarda_NotificacionOP(row("nro_op"), Carpeta)
                Else
                    'Elimina Soportes de los Anulados
                    rutas = Dir(Carpeta & "\" & row("nro_op") & "_*.PDF")
                    Do While rutas <> ""
                        File.Delete(Carpeta & "\" + rutas)
                        rutas = Dir()
                    Loop
                End If
            Next

            Return True

        Catch ex As Exception
            fn_InsertaExcepcion(0, 0, "MANEJADOR DE ARCHIVOS", "fn_Guarda_OP: " & ex.Message)
            'Mensaje.MuestraMensaje("Funciones", ex.Message, Mensaje.TipoMsg.Falla)
            Return False
        End Try
    End Function

    Public Shared Sub fn_Guarda_SoporteOP(ByVal nro_op As Integer, ByVal Carpeta As String, ByVal usuario As String, ByVal Contraseña As String)
        Dim stream As FileStream
        Dim results As Byte()

        Dim rsExec As ReportExecutionService.ReportExecutionService = New ReportExecutionService.ReportExecutionService()
        rsExec.Credentials = New NetworkCredential(usuario, Contraseña, "GMX.COM.MX") ' System.Net.CredentialCache.DefaultCredentials

        Dim reportOP As String = "/ReportesGMX/SoporteOP"
        rsExec.LoadReport(reportOP, Nothing)
        Dim executionParams As ReportExecutionService.ParameterValue()
        executionParams = New ReportExecutionService.ParameterValue(0) {}
        executionParams(0) = New ReportExecutionService.ParameterValue()
        executionParams(0).Name = "nro_op"

        executionParams(0).Value = nro_op
        rsExec.SetExecutionParameters(executionParams, "es-MX")
        Dim warnings As ReportExecutionService.Warning() = Nothing
        Dim streamIDs As String() = Nothing

        results = rsExec.Render("PDF", Nothing, vbNullString, vbNullString, vbNullString, warnings, Nothing)

        stream = File.OpenWrite(Carpeta & "\" & nro_op & "_Soporte_Reas.PDF")
        stream.Write(results, 0, results.Length)
        stream.Close()
    End Sub

    Public Shared Sub fn_Guarda_CobranzaOP(ByVal nro_op As String, ByVal Carpeta As String, ByVal usuario As String, ByVal Contraseña As String)
        Dim dtCobranza As DataTable = New DataTable
        Dim stream As FileStream
        Dim results As Byte()

        Dim rsExec As ReportExecutionService.ReportExecutionService = New ReportExecutionService.ReportExecutionService()
        rsExec.Credentials = New NetworkCredential(usuario, Contraseña, "GMX.COM.MX") ' System.Net.CredentialCache.DefaultCredentials

        Dim reportOP As String = "/ReportesGMX/CobranzaOP"
        rsExec.LoadReport(reportOP, Nothing)
        Dim executionParams As ReportExecutionService.ParameterValue()
        executionParams = New ReportExecutionService.ParameterValue(2) {}
        executionParams(0) = New ReportExecutionService.ParameterValue()
        executionParams(0).Name = "str_pol"
        executionParams(1) = New ReportExecutionService.ParameterValue()
        executionParams(1).Name = "nro_endoso"
        executionParams(2) = New ReportExecutionService.ParameterValue()
        executionParams(2).Name = "nro_op"

        'Soporte de Cobranzas de la OP
        fn_Consulta("spS_PolizaCobXOP '" & nro_op & "'", dtCobranza)

        For Each row In dtCobranza.Rows
            If row("sn_financiado") = 0 And (row("sn_pago") <> 0 Or row("sn_recargo") <> 0) Then
                executionParams(0).Value = "'" & row("poliza") & "'"
                executionParams(1).Value = row("nro_endoso")
                executionParams(2).Value = row("nro_op")

                rsExec.SetExecutionParameters(executionParams, "es-MX")
                Dim warnings As ReportExecutionService.Warning() = Nothing
                Dim streamIDs As String() = Nothing

                results = rsExec.Render("PDF", Nothing, vbNullString, vbNullString, vbNullString, warnings, Nothing)

                stream = File.OpenWrite(Carpeta & "\" & nro_op & "_Cob_" & row("poliza") & "-" & row("nro_endoso") & ".PDF")
                stream.Write(results, 0, results.Length)
                stream.Close()
            End If
        Next

    End Sub

    Public Shared Sub fn_Guarda_AsientoOP(ByVal nro_op As String, ByVal Carpeta As String, ByVal usuario As String, ByVal Contraseña As String)
        Dim dtAsiento As DataTable = New DataTable
        Dim stream As FileStream
        Dim results As Byte()

        Dim rsExec As ReportExecutionService.ReportExecutionService = New ReportExecutionService.ReportExecutionService()
        rsExec.Credentials = New NetworkCredential(usuario, Contraseña, "GMX.COM.MX") ' System.Net.CredentialCache.DefaultCredentials

        Dim reportOP As String = "/ReportesGMX/AsientoOP"
        rsExec.LoadReport(reportOP, Nothing)
        Dim executionParams As ReportExecutionService.ParameterValue()
        executionParams = New ReportExecutionService.ParameterValue(2) {}
        executionParams(0) = New ReportExecutionService.ParameterValue()
        executionParams(0).Name = "nro_op"
        executionParams(1) = New ReportExecutionService.ParameterValue()
        executionParams(1).Name = "nro_asiento"
        executionParams(2) = New ReportExecutionService.ParameterValue()
        executionParams(2).Name = "nro_recibo"

        'Soporte de Cobranzas de la OP
        fn_Consulta("spS_AsientosXOP '" & nro_op & "'", dtAsiento)

        For Each row In dtAsiento.Rows
            executionParams(0).Value = row("nro_op")
            executionParams(1).Value = row("nro_asiento")
            executionParams(2).Value = row("nro_recibo")

            rsExec.SetExecutionParameters(executionParams, "es-MX")
            Dim warnings As ReportExecutionService.Warning() = Nothing
            Dim streamIDs As String() = Nothing

            results = rsExec.Render("PDF", Nothing, vbNullString, vbNullString, vbNullString, warnings, Nothing)

            stream = File.OpenWrite(Carpeta & "\" & nro_op & "_Asiento_" & row("nro_asiento") & ".PDF")
            stream.Write(results, 0, results.Length)
            stream.Close()
        Next

    End Sub

    Public Shared Function fn_Guarda_NotificacionOP(ByVal nro_op As Integer, ByVal Carpeta As String) As Boolean
        Try
            Dim dtOrden As DataTable = New DataTable
            Dim results As Byte() = Nothing
            Dim stream As FileStream

            fn_Consulta("spS_IntermediarioOP " & nro_op, dtOrden)

            If dtOrden.Rows.Count > 0 Then

                If Len(dtOrden.Rows(0)("html")) > 0 Then
                    Using ms As MemoryStream = New MemoryStream()
                        Dim pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(dtOrden.Rows(0)("html"), PdfSharp.PageSize.A4)
                        pdf.Save(ms)
                        results = ms.ToArray()
                    End Using

                    stream = File.OpenWrite(Carpeta & "\" & nro_op & "_Notificacion.PDF")

                    stream.Write(results, 0, results.Length)
                    stream.Close()
                End If
            End If

            Return True
        Catch ex As Exception
            fn_InsertaExcepcion(0, 0, "MANEJADOR DE ARCHIVOS", "fn_Guarda_NotificacionOP: " & ex.Message)
            Return False
        End Try
    End Function

    Public Shared Function fn_ObtieneMes(ByVal Mes As Integer) As String
        fn_ObtieneMes = "Undefined"
        Select Case Mes
            Case 1
                fn_ObtieneMes = "Enero"
            Case 2
                fn_ObtieneMes = "Febrero"
            Case 3
                fn_ObtieneMes = "Marzo"
            Case 4
                fn_ObtieneMes = "Abril"
            Case 5
                fn_ObtieneMes = "Mayo"
            Case 6
                fn_ObtieneMes = "Junio"
            Case 7
                fn_ObtieneMes = "Julio"
            Case 8
                fn_ObtieneMes = "Agosto"
            Case 9
                fn_ObtieneMes = "Septiembre"
            Case 10
                fn_ObtieneMes = "Octubre"
            Case 11
                fn_ObtieneMes = "Noviembre"
            Case 12
                fn_ObtieneMes = "Diciembre"
        End Select
    End Function

    Public Shared Function fn_ObtieneUsuarioRol(cod_rol As Integer, ByRef dtConsulta As DataTable, Optional ByVal intDefault As Integer = 0, Optional ByVal cod_usuario As String = vbNullString) As DataTable

        fn_Consulta("spS_UsuarioXRol " & cod_rol & "," & intDefault & ",'" & cod_usuario & "'", dtConsulta)

        Return dtConsulta
    End Function

    Public Shared Function FormatoCorreo(TipoFormato As Integer, strNumOrds As String, UsuSol As String, cod_rol As Integer, Motivo As String, Optional ByVal sn_urgente As Integer = 0) As String
        Dim strBody As String = ""
        Dim dtConsulta As DataTable = New DataTable
        Dim strDestinatario As String = ""

        If fn_ObtieneUsuarioRol(cod_rol, dtConsulta, -1).Rows.Count > 0 Then
            strDestinatario = dtConsulta.Rows(0)("usuario")
        End If

        fn_Consulta("spS_FormatoFirmas " & TipoFormato & ",'" & UsuSol & "','" & strDestinatario & "','" & strNumOrds & "','" & Motivo & "'," & Cons.ReaseguroLocal & "," & cod_rol & "," & sn_urgente, dtConsulta)

        For Each row In dtConsulta.Rows
            strBody = strBody & row("fragmento")
        Next

        Return strBody

    End Function

    'JJIMENEZ
    Public Shared Function ObtenerDatos(ByVal sProcedimiento As String, Optional ByVal oParametros As Dictionary(Of String, Object) = Nothing) As DataSet

        Dim oDatos As DataSet

        Dim sConexion As String = String.Empty

        Dim oDataAdapter As SqlDataAdapter

        Dim oConexion As SqlConnection

        Dim oComando As SqlCommand

        Try

            oDatos = New DataSet
            oDataAdapter = New SqlDataAdapter
            oComando = New SqlCommand

            sConexion = ConfigurationManager.ConnectionStrings("CadenaConexion").ConnectionString
            oConexion = New SqlConnection(sConexion)

            oComando = New SqlCommand(sProcedimiento, oConexion)
            oComando.CommandType = CommandType.StoredProcedure
            oComando.CommandTimeout = 120000 'VZAVALETA_10290_CC

            If Not oParametros Is Nothing Then

                For Each p As KeyValuePair(Of String, Object) In oParametros
                    oComando.Parameters.AddWithValue(String.Format("@{0}", p.Key), p.Value)
                Next

            End If

            oDataAdapter = New SqlDataAdapter(oComando)

            oDataAdapter.Fill(oDatos)

        Catch ex As Exception
            oDatos = Nothing
        End Try

        ObtenerDatos = oDatos

    End Function


    Public Shared Function ObtenerDatosOut(ByVal sProcedimiento As String, Optional ByVal oParametros As Dictionary(Of String, Object) = Nothing, Optional parametros As List(Of SqlParameter) = Nothing) As SqlCommand

        Dim oDatos As DataSet

        Dim sConexion As String = String.Empty

        Dim oDataAdapter As SqlDataAdapter

        Dim oConexion As SqlConnection

        Dim oComando As SqlCommand

        Try

            oDatos = New DataSet
            oDataAdapter = New SqlDataAdapter
            oComando = New SqlCommand

            sConexion = ConfigurationManager.ConnectionStrings("CadenaConexion").ConnectionString
            oConexion = New SqlConnection(sConexion)


            oConexion.Open()
            oComando = New SqlCommand(sProcedimiento, oConexion)
            oComando.CommandType = CommandType.StoredProcedure

            If Not oParametros Is Nothing Then

                For Each p As KeyValuePair(Of String, Object) In oParametros
                    oComando.Parameters.AddWithValue(String.Format("@{0}", p.Key), p.Value)
                Next

            End If

            For Each _parametro As SqlParameter In parametros
                oComando.Parameters.Add(_parametro)

            Next

            oDataAdapter = New SqlDataAdapter(oComando)

            oComando.ExecuteNonQuery()



            oComando.Connection.Close()
            oConexion.Close()


        Catch ex As Exception
            ObtenerDatosOut = Nothing
        End Try

        ObtenerDatosOut = oComando

    End Function



    'JLHERNANDEZ

    Public Shared Function FormatearFecha(_fecha As String, FormatoFecha As enumFormatoFecha) As String
        Try
            _fecha = _fecha.Substring(0, 10)

            _fecha = _fecha.Substring(0, 2) + "/" + _fecha.Substring(3, 2) + "/" + _fecha.Substring(6, 4)

            Dim Fecha As DateTime = Convert.ToDateTime(_fecha)
            Dim strFecha As String = ""





            Select Case FormatoFecha
                Case enumFormatoFecha.DDMMYYYY
                    strFecha = String.Concat(Fecha.Day.ToString().PadLeft(2, "0"), "", Fecha.Month.ToString().PadLeft(2, "0"), "", Fecha.Year)
                    Return strFecha
                Case enumFormatoFecha.MMDDYYYY

                    strFecha = String.Concat(Fecha.Month.ToString().PadLeft(2, "0"), "", Fecha.Day.ToString().PadLeft(2, "0"), "", Fecha.Year)
                    Return strFecha
                Case enumFormatoFecha.YYYYMMDD

                    strFecha = String.Concat(Fecha.Year, "", Fecha.Month.ToString().PadLeft(2, "0"), "", Fecha.Day.ToString().PadLeft(2, "0"))
                    Return strFecha

                Case Else
                    Return ""
            End Select

        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Shared Function agregaTodos(ByRef dt As DataTable) As DataTable
        Try
            Dim dtN As New DataTable

            For Each col As DataColumn In dt.Columns
                dtN.Columns.Add(col.ColumnName, col.DataType)
            Next

            'If dtN.Columns(0).DataType.ToString() = "System.Decimal" Then

            'End If

            dtN.Rows.Add(-1, "TODOS")
            dtN.Merge(dt)

            Return dtN
        Catch ex As Exception
            Mensaje.MuestraMensaje("Exepcion", ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try
    End Function

    Public Shared Function fn_Parametro_Directo(ByVal Parametro As Integer) As String

        fn_Parametro_Directo = fn_EjecutaStr("spS_cPAR_Parametros " & Parametro & " ")

        Return fn_Parametro_Directo
    End Function

End Class





Imports System.Data
Imports System.IO
Imports System.Net.Mail
Imports Mensaje
Imports System.Diagnostics '>VZAVALETA_10290_CC7_PDF

Partial Class Siniestros_EnviosPagosInternacionales
    Inherits System.Web.UI.Page

    Public Property arrOPS() As String()
        Get
            Return Session("arrOPS")
        End Get
        Set(ByVal value As String())
            Session("arrOPS") = value
        End Set
    End Property

    Public Property tablas As String
        Get
            Return Session("tablas")
        End Get
        Set(ByVal value As String)
            Session("tablas") = value
        End Set
    End Property

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            arrOPS = Nothing
            tablas = ""
            'txt_nro_op.Text = "320000"
            'txt_nro_op_ini.Text = "320000"
            'txt_nro_op_fin.Text = "999999"
        End If

    End Sub

    Protected Sub btn_BuscarOP_Click(sender As Object, e As EventArgs) Handles btn_BuscarOP.Click

        Try

            Dim oDatos As DataSet
            Dim oTabla As DataTable
            Dim oParametros As New Dictionary(Of String, Object)
            Dim Num_Lote As String
            Dim Fondos As String

            If ValidaFiltros() Then
                oParametros.Add("Accion", 1)
                'If txt_nro_op.Text.Length <> 0 Then
                '    oParametros.Add("nro_op", txt_nro_op.Text)
                'End If
                If txt_nro_op_ini.Text.Length <> 0 Then
                    oParametros.Add("nro_op_desde", txt_nro_op_ini.Text)
                End If
                If txt_nro_op_fin.Text.Length <> 0 Then
                    oParametros.Add("nro_op_hasta", txt_nro_op_fin.Text)
                End If

                If txt_siniestro.Text.Length <> 0 Then
                    oParametros.Add("nro_stro", txt_siniestro.Text)
                End If
                If txt_beneficiario.Text.Length <> 0 Then
                    oParametros.Add("beneficiario", txt_beneficiario.Text)
                End If

                oDatos = Funciones.ObtenerDatos("sp_consulta_para_envio_pago_internacional", oParametros)

                oTabla = oDatos.Tables(0)

                If oTabla.Rows.Count > 0 Then
                    AbrirExplorador.Visible = True
                    btnEnviar.Visible = True
                    btn_Imprimir.Visible = True
                    btn_Ninguna.Visible = True
                    btn_Todas.Visible = True
                Else
                    AbrirExplorador.Visible = False
                    btnEnviar.Visible = False
                    btn_Imprimir.Visible = False
                    btn_Ninguna.Visible = False
                    btn_Todas.Visible = False
                End If




                grd.DataSource = oTabla
                grd.DataBind()
            End If

        Catch ex As Exception
            MuestraMensaje("Exception", "BuscarOP: " & ex.Message, TipoMsg.Falla)

        End Try
    End Sub




    Protected Sub grd_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grd.RowCommand
        'Dim bflag As Boolean
        'If bflag Then
        '    bflag.ToString()
        'End If

        If e.CommandName = "comVer" Then
            Dim index As Integer
            Dim nroOP As Integer

            e.CommandArgument.ToString()

            index = CInt(e.CommandArgument.ToString())
            nroOP = CInt(grd.DataKeys(index).Value.ToString())
            generaReporte(nroOP)
            'bflag = True
            Exit Sub
        End If


        If e.CommandName = "comEnviar" Then
            Dim indice As Integer
            Dim nroOP As Integer

            e.CommandArgument.ToString()
            indice = CInt(e.CommandArgument.ToString())
            nroOP = CInt(grd.DataKeys(indice).Value.ToString())

            If hidOPmail.Value = "" Then
                hidOPmail.Value = nroOP
            End If
            Funciones.AbrirModal("#mailPI")
            'bflag = True
            Exit Sub
        End If
    End Sub


    Private Function enviarMail(destinatarios As String, copias As String, asunto As String) As Boolean
        'Dim oDatos As DataSet
        'Dim oTabla As DataTable
        Dim pdf = New reportePDF
        Dim oParametros As New Dictionary(Of String, Object)
        'Dim rutaAdjunto As String
        Try

            'pdf.Cod_usuario = Master.cod_usuario.ToString()
            'pdf.Nro_ops = arrOPS
            'pdf.ReportePagosInter(False)

            'rutaAdjunto = pdf.RutaArchivo_correo

            ' oParametros.Add("nroOP", nroOP)
            oParametros.Add("para", destinatarios)
            oParametros.Add("cc", copias)
            oParametros.Add("asunto", asunto)
            oParametros.Add("tablas", tablas)



            Funciones.ObtenerDatos("usp_envio_mail_pago_inter", oParametros)
            'oDatos = Funciones.ObtenerDatos("usp_solicitud_aut_mail", oParametros)
            'oTabla = oDatos.Tables(0)
            Return True

        Catch ex As Exception
            MuestraMensaje("Exception", "No fue posible enviar el correo electrónico. Error: " & ex.Message, TipoMsg.Falla)
            Return False
        End Try

    End Function

    Private Sub generaReporte(nroOP As Integer)
        Dim ws As New ws_Generales.GeneralesClient
        Dim server As String = ws.ObtieneParametro(3)
        Dim RptFilters As String
        RptFilters = "&nroOP=" & nroOP.ToString()
        server = Replace(Replace(server, "@Reporte", "OP_PagoInternacional"), "@Formato", "PDF")
        server = Replace(server, "ReportesGMX_DESA", "ReportesOPSiniestros_DESA")
        server = server & RptFilters
        Funciones.EjecutaFuncion("window.open('" & server & "');")

    End Sub

    Private Sub btnCancEmail_Click(sender As Object, e As EventArgs) Handles btnCancEmail.Click
        limpCtrlCerrarModal()
    End Sub

    Private Sub btnAcepmEmail_Click(sender As Object, e As EventArgs) Handles btnAcepmEmail.Click
        Try
            If txtPara.Text.Trim() <> "" Then
                If txtAsunto.Text.Trim() <> "" Then
                    If enviarMail(txtPara.Text.Trim(), txtCC.Text.Trim(), txtAsunto.Text.Trim()) Then
                        limpCtrlCerrarModal()
                        tablas = ""
                        MuestraMensaje("Correo Electrónico", "El correo ha sido enviado", TipoMsg.Advertencia)
                    End If
                Else
                    MuestraMensaje("Validación", "El campo ""Asunto"" no puede estar vacío", TipoMsg.Advertencia)
                End If
            Else
                MuestraMensaje("Validación", "El campo ""Para"" no puede estar vacío", TipoMsg.Advertencia)
            End If
        Catch ex As Exception

        End Try
    End Sub

    'Public Sub EnviaEmailUsuarios(UserEmail As String, NumeroReporte As String, TipoFormato As String)
    '    Dim message As MailMessage
    '    message = New MailMessage()
    '    Dim mBody As String = ""
    'End Sub

    Public Sub limpCtrlCerrarModal()
        txtPara.Text = ""
        txtCC.Text = ""
        txtAsunto.Text = ""
        Funciones.CerrarModal("#mailPI")
    End Sub

    Private Sub btn_Todas_Click(sender As Object, e As EventArgs) Handles btn_Todas.Click
        Try

            For Each row In grd.Rows
                Dim chkImp As CheckBox = TryCast(row.FindControl("chk_Print"), CheckBox)
                If chkImp.Enabled = True Then
                    chkImp.Checked = True
                End If
            Next
        Catch ex As Exception

            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)

        End Try
    End Sub

    Private Sub btn_Ninguna_Click(sender As Object, e As EventArgs) Handles btn_Ninguna.Click
        Try
            For Each row In grd.Rows
                Dim chkImp As CheckBox = TryCast(row.FindControl("chk_Print"), CheckBox)
                If chkImp.Enabled = True Then
                    chkImp.Checked = False
                End If

            Next

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)

        End Try

    End Sub

    Private Sub btn_Imprimir_Click(sender As Object, e As EventArgs) Handles btn_Imprimir.Click
        Dim dtSelec As DataTable
        Dim folioRep As Integer
        Dim dtPagosInter As DataTable
        Dim archivos() As String = Nothing
        Dim Index As Integer = 0
        Dim pdf = New reportePDF
        Dim RutaArchivo As String()

        dtSelec = obtenerSeleccionadosImp()
        Dim strFoliosRep As String = ""


        Try
            If dtSelec.Rows.Count > 0 Then
                ReDim archivos(dtSelec.Rows.Count - 1)
                For Each row In dtSelec.Rows
                    archivos(Index) = row(0).ToString()
                    Index = Index + 1
                    'Funciones.fn_Consulta("sp_consulta_para_envio_pago_internacional @Accion = 2, @nro_op_desde = " & row(0).ToString(), dtPagosInter)
                Next
            Else
                Index = 0
                MuestraMensaje("Validación", "No existen registros seleccionados para imprimir", TipoMsg.Advertencia)
                Exit Sub
            End If

            Dim archivo As String = "\Ordenes de Pago Pago Inter_SISTRAN.pdf"


            If archivos.Length > 0 AndAlso Index <> 0 Then
                pdf.Cod_usuario = Master.cod_usuario.ToString()
                pdf.Nro_ops = archivos
                pdf.ReportePagosInter(True)

                '>VZAVALETA_10290_CC7_PDF 
                RutaArchivo = pdf.RutaArchivo_correo.Split("\")

                Dim max As Integer = RutaArchivo.Length
                Dim pathToFiles As String

                archivo = RutaArchivo(max - 1)
                pathToFiles = Server.MapPath("/PDF")

                If File.Exists(pathToFiles + "\" + archivo) Then
                    File.Delete(pathToFiles + "\" + archivo)
                End If

                File.Copy(pdf.RutaArchivo_correo, pathToFiles + "\" + archivo)

                MuestraMensaje("Impresión", "La Impresión fue correcta", TipoMsg.Advertencia)

                Response.Redirect("../PDF/" + archivo)
                'Response.Redirect("../Siniestros/EnviosPagosInternacionales.aspx")

                '<VZAVALETA_10290_CC7_PDF   
            Else
                Index = 0
                MuestraMensaje("Validación", "Error al imprimir el PDF", TipoMsg.Falla)
            End If


        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
        End Try
    End Sub




    Private Function obtenerSeleccionadosImp() As DataTable

        Try
            Dim dtSel As New DataTable
            Dim add As Boolean
            Dim indexTabla As Integer
            add = True
            indexTabla = 0


            If grd.Rows.Count = 0 Then
                MuestraMensaje("Validacion", "No existen registros en la tabla", TipoMsg.Advertencia)
                Return Nothing
                Exit Function
            End If


            dtSel.Columns.Add("NRO_OP")


            For index = 0 To grd.Rows.Count - 1
                Dim seleccionado As CheckBox
                seleccionado = grd.Rows(index).FindControl("chk_Print")
                If seleccionado.Checked Then
                    dtSel.Rows.Add()
                End If
            Next

            For Each row As GridViewRow In grd.Rows
                For i As Integer = 0 To row.Cells.Count - 1
                    Dim selecc As CheckBox
                    selecc = row.Cells(i).FindControl("chk_Print")

                    If selecc.Checked Then

                        If i = 1 Then
                            dtSel.Rows(indexTabla)(i - 1) = row.Cells(i).Text
                            add = True
                        End If
                    Else
                        add = False
                    End If
                Next
                If add Then
                    indexTabla = indexTabla + 1
                End If
            Next

            Return dtSel
        Catch ex As Exception
            MuestraMensaje("Exception", "obtenerSeleccionados:  " & ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try

    End Function

    Private Sub btnEnviar_Click(sender As Object, e As EventArgs) Handles btnEnviar.Click
        Dim dtSelec As DataTable
        Dim dtPagosInter As DataTable
        Dim archivos() As String = Nothing
        Dim Index As Integer = 0
        Dim pdf = New reportePDF
        ' Dim tablas As String = ""

        dtSelec = obtenerSeleccionadosImp()

        Try
            If dtSelec.Rows.Count > 0 Then
                ReDim archivos(dtSelec.Rows.Count - 1)
                For Each row In dtSelec.Rows
                    archivos(Index) = row(0).ToString()
                    Index = Index + 1
                    Funciones.fn_Consulta("sp_consulta_para_envio_pago_internacional @Accion = 2, @nro_op_desde = " & row(0).ToString(), dtPagosInter)

                    'arma tablas para cuerpo de correo

                    tablas = tablas + armaHTML(dtPagosInter)

                Next
            Else
                Index = 0
                MuestraMensaje("Validación", "No existen registros seleccionados para enviar", TipoMsg.Advertencia)
                Exit Sub
            End If


            If archivos.Length > 0 AndAlso Index <> 0 Then
                'arrOPS = archivos  'se comenta porque ya no manda el pdf en el correo
                'pdf.Nro_ops = archivos
                Funciones.AbrirModal("#mailPI")
            End If


        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)

        End Try
    End Sub

    Private Function ValidaFiltros() As Boolean
        Dim nroOpDesde As Double
        Dim nroOpHasta As Double
        Dim nroChequeDesde As Double
        Dim nroChequeHasta As Double

        ValidaFiltros = True

        If Trim(txt_nro_op.Text) = "" Then
            If Trim(txt_nro_op_fin.Text) = "" Then
                If Trim(txt_siniestro.Text) = "" Then
                    If Trim(txt_beneficiario.Text) = "" Then
                        MuestraMensaje("Validación", "Debe elegir al menos un filtro para la consulta", TipoMsg.Advertencia)
                        ValidaFiltros = False
                    End If
                End If
            Else
                MuestraMensaje("Validación", "Debe seleccionar Núm OP", TipoMsg.Advertencia)
                ValidaFiltros = False
            End If
        Else
            If Not Trim(txt_nro_op_fin.Text) = "" Then
                nroOpDesde = Convert.ToDouble(Trim(txt_nro_op.Text))
                nroOpHasta = Convert.ToDouble(Trim(txt_nro_op_fin.Text))

                If nroOpDesde > nroOpHasta Then
                    MuestraMensaje("Validación", "Número OP no debe ser mayor que Número de OP hasta", TipoMsg.Advertencia)
                    ValidaFiltros = False
                End If
            End If
        End If
    End Function

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        txt_nro_op.Text = ""
        txt_nro_op_fin.Text = ""
        txt_beneficiario.Text = ""
        txt_nro_op_ini.Text = ""
        txt_siniestro.Text = ""
    End Sub

    Private Function armaHTML(dtPagosInter As DataTable) As String
        Dim html As String = ""

        html = html + "<table style='background-color: #ffeb9f; border: 1px solid #000000;' border='1' cellspacing='0' cellpadding='3'  align='left' width='100%'>"
        html = html + "<tbody>"
        html = html + "<tr>"
        html = html + "<td colspan='5' >&nbsp;</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td rowspan='2' style='width: 60% !important; '>NOMBRE DEL BENEFICIARIO: " + dtPagosInter.Rows(0).Item("nom_benef").ToString() + "</td>"
        html = html + "<td style='width: 12%; text-align: center; background-color: #ffffff;'>MONEDA</td>"
        html = html + "<td style='width: 14%; text-align: center; background-color: #ffffff;'>MONTO</td>"
        html = html + "<td style='width: 14%; text-align: center; background-color: #ffffff;'>ORDEN DE PAGO</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td style='text-align: center; background-color: #ffffff;'> " + dtPagosInter.Rows(0).Item("moneda").ToString() + "</td>"
        html = html + "<td style='text-align: center; background-color: #ffffff;'>$&nbsp;&nbsp;&nbsp;&nbsp; " + dtPagosInter.Rows(0).Item("monto").ToString() + "</td>"

        html = html + "<td style='text-align: center; background-color: #ffffff;'>" + dtPagosInter.Rows(0).Item("nro_op").ToString() + "</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td rowspan='2' style='background-color: #ffffff;'>DOMICILIO DEL BENEFICIARIO: " + dtPagosInter.Rows(0).Item("dom_benef").ToString() + "</td>"
        html = html + "<td colspan='3'>RFC BENEFICIARIO: " + dtPagosInter.Rows(0).Item("rfc_benef").ToString() + "</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td colspan='4' style='background-color: #ffffff;'>CUENTA: " + dtPagosInter.Rows(0).Item("cuenta").ToString() + "</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td>BANCO: " + dtPagosInter.Rows(0).Item("banco").ToString() + "</td>"
        html = html + "<td colspan='3'>ABA &oacute; ROUTING: " + dtPagosInter.Rows(0).Item("aba_routing").ToString() + "</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td rowspan='2' style='background-color: #ffffff;'>DOMICILIO DEL BANCO: " + dtPagosInter.Rows(0).Item("dom_banco").ToString() + "</td>"
        html = html + "<td colspan='3'>SWIFT: " + dtPagosInter.Rows(0).Item("swift").ToString() + "</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td colspan='3' style='background-color: #ffffff;'>TRANSIT: " + dtPagosInter.Rows(0).Item("transit").ToString() + "</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td rowspan='2'>CONCEPTO DE PAGO: " + dtPagosInter.Rows(0).Item("cpto_pago").ToString() + "</td>"
        html = html + "<td colspan='3'>N&Uacute;MERO DE BANCO: " + dtPagosInter.Rows(0).Item("nro_banco").ToString() + "</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td colspan='3' style='background-color: #ffffff;'>IBAN: " + dtPagosInter.Rows(0).Item("iban").ToString() + "</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td colspan='4'>&nbsp;</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td rowspan='3' style='background-color: #ffffff;'>TRIANGULADO: " + dtPagosInter.Rows(0).Item("triangulado").ToString() + "</td>"
        html = html + "<td colspan='3' style='background-color: #ffffff;'>BANCO: " + dtPagosInter.Rows(0).Item("banco_trian").ToString() + "<br /></td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td colspan='3' >CUENTA: " + dtPagosInter.Rows(0).Item("cuenta_trian").ToString() + "</td>"
        html = html + "</tr>"
        html = html + "<tr>"
        html = html + "<td colspan='3'style='background-color: #ffffff;'>ABA &oacute; ROUTING: " + dtPagosInter.Rows(0).Item("aba_routing_trian").ToString() + "</td>"
        html = html + "</tr>"
        html = html + "</tbody>"
        html = html + "</table>"
        html = html + "<br/><br/><br/>"


        Return html
    End Function

    Private Sub AbrirExplorador_Click(sender As Object, e As EventArgs) Handles AbrirExplorador.Click
        Dim dtSelec As DataTable
        Dim folioRep As Integer
        Dim dtPagosInter As DataTable
        Dim archivos() As String = Nothing
        Dim Index As Integer = 0
        Dim pdf = New reportePDF
        Dim RutaArchivo As String

        dtSelec = obtenerSeleccionadosImp()
        Dim strFoliosRep As String = ""


        Try
            If dtSelec.Rows.Count > 0 Then
                ReDim archivos(dtSelec.Rows.Count - 1)
                For Each row In dtSelec.Rows
                    archivos(Index) = row(0).ToString()
                    Index = Index + 1
                    'Funciones.fn_Consulta("sp_consulta_para_envio_pago_internacional @Accion = 2, @nro_op_desde = " & row(0).ToString(), dtPagosInter)
                Next
            Else
                Index = 0
                MuestraMensaje("Validación", "No existen registros seleccionados para imprimir", TipoMsg.Advertencia)
                Exit Sub
            End If


            If archivos.Length > 0 AndAlso Index <> 0 Then
                pdf.Cod_usuario = Master.cod_usuario.ToString()
                pdf.Nro_ops = archivos
                pdf.ReportePagosInter(True)

                '>VZAVALETA_10290_CC7_PDF  

                RutaArchivo = Replace(pdf.RutaArchivo_correo, " ", "%20")
                RutaArchivo = Replace(RutaArchivo, "\", "/")

                'Funciones.EjecutaFuncion("window.open(RutaArchivo, '_blank');", "PDF")

                Process.Start(pdf.Ruta_explorador)


                '<VZAVALETA_10290_CC7_PDF               
            Else
                Index = 0
                MuestraMensaje("Validación", "Error al imprimir el PDF", TipoMsg.Falla)
            End If


        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)

        End Try
    End Sub
End Class


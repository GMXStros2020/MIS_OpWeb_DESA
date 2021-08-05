Imports System.Data
Imports System.Data.SqlClient
Imports Mensaje
Imports Funciones
Imports System.IO

Partial Class Siniestros_FirmasElectronicas
    Inherits System.Web.UI.Page

    Private Enum Operacion
        Ninguna
        Consulta
    End Enum

    Private Enum TipoFormato As Integer
        Autorizacion = 0
        Rechazo
        VistoBueno
        Urgente
    End Enum

    Private Enum eTipoModulo As Integer
        Ninguno = 0
        OrdenPagoSiniestros = 1
        AutorizacionesVarias = 2
        CircuitoOrdenesPago = 3
    End Enum

    Public Property dtConsulta() As DataTable
        Get
            Return Session("dtConsulta")
        End Get
        Set(ByVal value As DataTable)
            Session("dtConsulta") = value
        End Set
    End Property
    Public Property dtEnvios() As DataTable
        Get
            Return Session("dtEnvios")
        End Get
        Set(ByVal value As DataTable)
            Session("dtEnvios") = value
        End Set
    End Property


    Public Property dtOrdenPago() As DataTable
        Get
            Return Session("dtOrdenPago")
        End Get
        Set(ByVal value As DataTable)
            Session("dtOrdenPago") = value
        End Set
    End Property

    Public Property dtAutorizaciones() As DataTable
        Get
            Return Session("dtAutorizaciones")
        End Get
        Set(ByVal value As DataTable)
            Session("dtAutorizaciones") = value
        End Set
    End Property
    Public Property dtAutoriza() As DataTable
        Get
            Return Session("dtAutoriza")
        End Get
        Set(ByVal value As DataTable)
            Session("dtAutoriza") = value
        End Set
    End Property
    Public Property dtCancela() As DataTable
        Get
            Return Session("dtCancela")
        End Get
        Set(ByVal value As DataTable)
            Session("dtCancela") = value
        End Set
    End Property

    Public Property dtCorreos() As DataTable
        Get
            Return Session("dtCorreos")
        End Get
        Set(ByVal value As DataTable)
            Session("dtCorreos") = value
        End Set
    End Property
    Public Property dtToken() As DataTable
        Get
            Return Session("dtToken")
        End Get
        Set(ByVal value As DataTable)
            Session("dtToken") = value
        End Set
    End Property

    Private Sub OrdenPago_FirmasElectronicas_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then
                Master.Titulo = "Autorizaciones Electrónicas"
                Master.cod_modulo = Cons.ModuloRea
                Master.cod_submodulo = Cons.SubModFirmas

                Master.InformacionGeneral()
                Master.EvaluaPermisosModulo()

                EdoControl(Operacion.Ninguna)
                dtOrdenPago = Nothing
                Funciones.LlenaCatDDL(cmbMoneda, "Mon")
                cmbMoneda.SelectedValue = -1

                Dim Params As String = Request.QueryString("NumOrds")
                If Params <> vbNullString Then
                    txt_NroOP.Text = Split(Params, "|")(0)
                    'cmbModuloOP.SelectedValue = Split(Params, "|")(1)
                    Master.cod_usuario = Split(Params, "|")(2)
                    chk_Todas.Checked = True
                End If

                If Len(txt_NroOP.Text) > 0 Then
                    Master.cod_usuario = Split(Context.User.Identity.Name, "|")(0)
                    btn_BuscaOP_Click(Me, Nothing)
                    Funciones.EjecutaFuncion("fn_EstadoFilas('grdOrdenPago', false);", "Filas")
                End If
            End If
            EstadoDetalleOrden()
            'Master.cod_usuario = "CLOPEZ"
            ValidaUsrFiltros()
        Catch ex As Exception
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "OrdenPago_FirmasElectronicas_Load: " & ex.Message)
        End Try
    End Sub
    Private Sub ValidaUsrFiltros()
        Dim ws As New ws_Generales.GeneralesClient

        Dim FiltrosUsuStr As String

        FiltrosUsuStr = ws.ObtieneParametro(Cons.TargetFiltrosAdminStro)

        If InStr(FiltrosUsuStr, Master.cod_usuario) Then
            'If Master.cod_usuario = "CLOPEZ" Or Master.cod_usuario = "AMEZA" Or Master.cod_usuario = "CREYES" Or Master.cod_usuario = "MMQUINTERO" Then
            chk_Todas.Visible = True
            chk_PorRevisar.Visible = True
            chk_Revisadas.Visible = True
        Else
            chk_Todas.Visible = False
            chk_PorRevisar.Visible = False
            chk_Revisadas.Visible = False
        End If
    End Sub

    Private Sub EstadoDetalleOrden()
        For Each row In grdOrdenPago.Rows
            If TryCast(row.FindControl("txt_Estado"), TextBox).Text = 1 Then
                CType(row.FindControl("div_ventana"), HtmlGenericControl).Attributes.Add("style", "display: none;")
            Else
                CType(row.FindControl("div_ventana"), HtmlGenericControl).Attributes.Add("style", "display: block;")
            End If
        Next
    End Sub


    Private Function ConsultaOrdenesPagoSiniestros(ByVal iTipoModulo As Integer) As DataTable

        Dim oParametros As New Dictionary(Of String, Object)


        Dim sFiltroOP As String = String.Empty
        Dim sFiltroPoliza As String = String.Empty
        Dim sFiltroUsuario As String = String.Empty
        Dim sFiltroEstatus As String = String.Empty
        Dim sFiltroFechaGeneracion As String = String.Empty
        Dim sFiltroFechaPago As String = String.Empty
        Dim sFiltroMonto As String = String.Empty
        Dim iStatusFirma As Integer = -1
        Dim sFiltroStro As String = ""
        Dim sFiltroBenef As String = ""
        Dim sFiltroFecDe As String = String.Empty
        Dim sFiltroFecHasta As String = String.Empty

        Dim FiltroBrokerCia As String = ""


        Dim intFirmas As Integer = 0

        Dim FiltroNatOP As String = ""


        Try



            sFiltroOP = IIf(Not String.IsNullOrWhiteSpace(txt_NroOP.Text.Trim), txt_NroOP.Text.Trim, 0)
                sFiltroUsuario = Funciones.ObtieneElementos(gvd_Usuario, "Usu", True)

            sFiltroUsuario = IIf(Not String.IsNullOrWhiteSpace(sFiltroUsuario), String.Format("AND t.cod_usuario IN ('{0}')", sFiltroUsuario), String.Empty)


            If IsDate(txtFechaGeneracionDesde.Text.Trim) And IsDate(txtFechaGeneracionHasta.Text.Trim) Then
                If CDate(txtFechaGeneracionDesde.Text.Trim) <= CDate(txtFechaGeneracionHasta.Text.Trim) Then
                    sFiltroFechaGeneracion = String.Format(" AND CONVERT(VARCHAR(10),fec_generacion,112) >= ''{0}'' AND CONVERT(VARCHAR(10),fec_generacion,112) <= ''{1}'' ", CDate(txtFechaGeneracionDesde.Text).ToString("yyyyMMdd"), CDate(txtFechaGeneracionHasta.Text).ToString("yyyyMMdd"))
                End If
            End If

            If IsDate(txtFechaPagoDesde.Text) And IsDate(txtFechaPagoHasta.Text) Then
                If CDate(txtFechaPagoDesde.Text) <= CDate(txtFechaPagoHasta.Text) Then
                    sFiltroFechaPago = String.Format(" AND CONVERT(VARCHAR(10),mop.fec_pago,112) >= ''{0}'' AND CONVERT(VARCHAR(10),mop.fec_pago,112) <= ''{1}'' ", CDate(txtFechaPagoDesde.Text).ToString("yyyyMMdd"), CDate(txtFechaPagoHasta.Text).ToString("yyyyMMdd"))
                End If
            End If


            If IsDate(fecFilter_De.Text) And IsDate(fecFilter_Hasta.Text) Then
                If CDate(fecFilter_De.Text) <= CDate(fecFilter_Hasta.Text) Then
                    sFiltroFecDe = CDate(fecFilter_De.Text).ToString("yyyy-MM-dd")
                    sFiltroFecHasta = CDate(fecFilter_Hasta.Text).ToString("yyyy-MM-dd")
                End If
            End If

            If IsNumeric(txtMontoDesde.Text.Trim) Then
                sFiltroMonto = String.Format(" AND mop.imp_total >= {0}", txtMontoDesde.Text.Trim)
            End If

            If IsNumeric(txtMontoHasta.Text.Trim) Then
                sFiltroMonto = String.Format("{0} AND mop.imp_total <= {1}", sFiltroMonto, txtMontoHasta.Text.Trim)
            End If

            If chk_Todas.Checked Then
                iStatusFirma = Cons.TipoFiltro.Todas
            ElseIf chk_PorRevisar.Checked Then
                iStatusFirma = Cons.TipoFiltro.PorRevisar
            ElseIf chk_Revisadas.Checked Then
                iStatusFirma = Cons.TipoFiltro.Revisadas
            ElseIf chk_Pendiente.Checked Then
                iStatusFirma = Cons.TipoFiltro.Pendientes
            ElseIf chk_Autorizada.Checked Then
                iStatusFirma = Cons.TipoFiltro.Firmadas
            ElseIf chk_Rechazadas.Checked Then
                iStatusFirma = Cons.TipoFiltro.Rechazadas
            ElseIf chk_FinalAut.Checked Then
                iStatusFirma = Cons.TipoFiltro.Autorizadas
            End If

            Dim valorMoneda As String = ""
            If cmbMoneda.SelectedItem.Text <> ". . ." Then valorMoneda = cmbMoneda.SelectedItem.Text

            If txtSiniestro.Text <> "" Then sFiltroStro = txtSiniestro.Text
            If txtAsegurado.Text <> "" Then sFiltroBenef = txtAsegurado.Text


            Dim ValorRol As Integer = 0
                ValorRol = ddlRolFilter.SelectedValue

            'Cambiar SP por original (usp_ObtenerOrdenPago_stro)
            fn_Consulta(String.Format("usp_ObtenerOrdenPago_stro_T '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}','{13}'",
                                              Cons.StrosTradicional,
                                              sFiltroOP,
                                              sFiltroMonto,
                                              sFiltroFechaGeneracion,
                                              sFiltroFechaPago,
                                              sFiltroUsuario,
                                              Master.cod_usuario,
                                              iStatusFirma,
                                              ValorRol,
                                               valorMoneda,
                                                sFiltroStro,
                                                sFiltroBenef,
                                                sFiltroFecDe,
                                                sFiltroFecHasta), dtOrdenPago)

            Return dtOrdenPago

            'End If

        Catch ex As Exception
            ConsultaOrdenesPagoSiniestros = Nothing
            Mensaje.MuestraMensaje(Master.Titulo, String.Format("ConsultaOrdenesPagoSiniestros Error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Function

    Private Sub EdoControl(ByVal intOperacion As Integer)
        Select Case intOperacion
            Case Operacion.Consulta
                txt_NroOP.Enabled = False
                cmbMoneda.Enabled = False
                txtAsegurado.Enabled = False
                txtFechaPagoDesde.Enabled = False
                txtFechaPagoHasta.Enabled = False
                txtFechaGeneracionDesde.Enabled = False
                txtFechaGeneracionHasta.Enabled = False
                txtMontoDesde.Enabled = False
                txtMontoHasta.Enabled = False

                gvd_Usuario.Enabled = False
                btn_AddUsuario.Visible = False
                'gvd_Estatus.Enabled = False
                'btn_AddEstatus.Visible = False
                BarraEstados(False)
                txtSiniestro.Enabled = False
                'JJIMENEZ
                'gvd_Broker.Enabled = False
                'btn_AddBroker.Visible = False

                'gvd_Compañia.Enabled = False
                'btn_AddCia.Visible = False

                'gvd_Poliza.Enabled = False
                'btn_AddPol.Visible = False

                'gvd_RamoContable.Enabled = False
                'btn_AddRamoContable.Visible = False

                'JJIMENEZ
                'chk_Devolucion.Enabled = False
                'chk_ConISR.Enabled = False
                'chk_SinISR.Enabled = False

                chk_Pendiente.Enabled = False
                chk_Autorizada.Enabled = False

                'chk_Solicitante.Enabled = False
                'chk_JefeDirecto.Enabled = False
                'chk_SubDirector.Enabled = False
                'chk_Director.Enabled = False
                'chk_DirectorGral.Enabled = False
                'chk_Tesoreria.Enabled = False
                'chk_Contabilidad.Enabled = False

                btn_BuscaOP.Visible = False

                btn_Todas.Visible = True
                btn_Ninguna.Visible = True

                btn_Imprimir.Visible = True
                btn_SelTodos.Visible = True

                btn_Firmar.Visible = True

                'If Master.Baja = 0 Then
                '    btn_Rechazar.Visible = False
                'Else
                '    btn_Rechazar.Visible = True
                'End If

                hid_Ventanas.Value = "1|0|1|1|1|1|1|1|"

            Case Operacion.Ninguna
                Funciones.LlenaGrid(grdOrdenPago, Nothing)
                txt_NroOP.Enabled = True
                cmbMoneda.Enabled = True
                txtAsegurado.Enabled = True
                txtFechaPagoDesde.Enabled = True
                txtFechaPagoHasta.Enabled = True
                txtFechaGeneracionDesde.Enabled = True
                txtFechaGeneracionHasta.Enabled = True
                txtMontoDesde.Enabled = True
                txtMontoHasta.Enabled = True
                LimpiaCtrls()
                BarraEstados(True)
                txtSiniestro.Enabled = True
                gvd_Usuario.Enabled = True
                btn_AddUsuario.Visible = True
                gvd_Usuario.DataSource = Nothing
                gvd_Usuario.DataBind()
                'gvd_Estatus.Enabled = True
                'btn_AddEstatus.Visible = True

                'JJIMENEZ
                'gvd_Broker.Enabled = True
                'btn_AddBroker.Visible = True

                'gvd_Compañia.Enabled = True
                'btn_AddCia.Visible = True

                'gvd_Poliza.Enabled = True
                'btn_AddPol.Visible = True

                'gvd_RamoContable.Enabled = True
                'btn_AddRamoContable.Visible = True

                'JJIMENEZ
                'chk_Devolucion.Enabled = True
                'chk_ConISR.Enabled = True
                'chk_SinISR.Enabled = True

                chk_Pendiente.Enabled = True
                chk_Autorizada.Enabled = True
                chk_Pendiente.Checked = False
                chk_Autorizada.Checked = False

                'Funciones.fn_Consulta("spS_RolUsuarioOP '" & Master.cod_usuario & "'", dtConsulta)
                'For Each row In dtConsulta.Rows
                '    Select Case row("cod_rol")
                '        Case Cons.TipoPersona.Solicitante
                '            chk_Solicitante.Enabled = True
                '        Case Cons.TipoPersona.JefeInmediato
                '            chk_JefeDirecto.Enabled = True
                '        Case Cons.TipoPersona.Subdirector
                '            chk_SubDirector.Enabled = True
                '        Case Cons.TipoPersona.Director
                '            chk_Director.Enabled = True
                '        Case Cons.TipoPersona.DirectorGeneral
                '            chk_DirectorGral.Enabled = True
                '        Case Cons.TipoPersona.Tesoreria
                '            chk_Tesoreria.Enabled = True
                '        Case Cons.TipoPersona.Contabilidad
                '            chk_Contabilidad.Enabled = True
                '    End Select
                'Next


                '''If Master.Consulta = 0 Then
                '''    btn_BuscaOP.Visible = False
                '''Else
                '''    btn_BuscaOP.Visible = True
                '''End If

                btn_Todas.Visible = False
                btn_Ninguna.Visible = False

                btn_Imprimir.Visible = False
                btn_SelTodos.Visible = False
                btn_Firmar.Visible = False
                btn_BuscaOP.Visible = True
                'btn_Rechazar.Visible = False
                'btn_Limpiar_Click(Nothing, )

                hid_Ventanas.Value = "0|1|1|1|1|1|1|1|"

                hid_rechazo.Value = 0

        End Select
    End Sub
    Private Sub BarraEstados(valor As Boolean)
        chk_Todas.Enabled = valor
        chk_PorRevisar.Enabled = valor
        chk_Revisadas.Enabled = valor
        chk_Pendiente.Enabled = valor
        chk_Autorizada.Enabled = valor
        chk_Rechazadas.Enabled = valor
        chk_FinalAut.Enabled = valor
        ddlRolFilter.Enabled = valor
    End Sub


    Private Sub LimpiaCtrls()
        txt_NroOP.Text = ""
        'cmbModuloOP.SelectedIndex = 0
        'cmbMoneda.SelectedIndex = 0
        ddlRolFilter.SelectedIndex = 0
        txtFechaGeneracionDesde.Text = ""
        txtFechaGeneracionHasta.Text = ""
        chk_Autorizada.Checked = False
        chk_PorRevisar.Checked = False
        chk_Revisadas.Checked = False
        chk_Todas.Checked = False
        chk_Pendiente.Checked = False
        chk_Rechazadas.Checked = False
        chk_FinalAut.Checked = False
        ddlRolFilter.Visible = False
        txtMontoDesde.Text = ""
        txtMontoHasta.Text = ""
        txtFechaPagoDesde.Text = ""
        txtFechaPagoHasta.Text = ""
        txtSiniestro.Text = ""
        txtAsegurado.Text = ""
        fecFilter_De.Text = ""
        fecFilter_Hasta.Text = ""



    End Sub

    Private Sub btn_BuscaOP_Click(sender As Object, e As EventArgs) Handles btn_BuscaOP.Click
        Try
            'If cmbModuloOP.SelectedValue > 0 Then
            If ValidaRadios() Then
                'Funciones.LlenaGrid(grdOrdenPago, ConsultaOrdenesPagoSiniestros(cmbModuloOP.SelectedValue))
                Funciones.LlenaGrid(grdOrdenPago, ConsultaOrdenesPagoSiniestros(Cons.StrosTradicional))

                If grdOrdenPago.Rows.Count > 0 Then
                    grdOrdenPago.PageIndex = 0

                    EdoControl(Operacion.Consulta)
                    DesHabilitaChecksFirma()
                    Funciones.EjecutaFuncion("fn_EstadoFilas('grdOrdenPago',true);")
                Else
                    Mensaje.MuestraMensaje(Master.Titulo, "La Consulta no devolvió resultados", TipoMsg.Advertencia)
                End If
            Else
                MuestraMensaje("Validación", "Debe elegir un filtro de Estatus de Firma", TipoMsg.Advertencia)
            End If
            'Else
            '    MuestraMensaje("Validación", "Debe elegir el tipo de módulo", TipoMsg.Advertencia)
            'End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_BuscaOP_Click: " & ex.Message)
        End Try
    End Sub
    Private Function ValidaRadios() As Boolean
        ValidaRadios = True
        If chk_Todas.Checked = False Then
            If chk_PorRevisar.Checked = False Then
                If chk_Revisadas.Checked = False Then
                    If chk_Pendiente.Checked = False Then
                        If chk_Autorizada.Checked = False Then
                            If chk_Rechazadas.Checked = False Then
                                If chk_FinalAut.Checked = False Then
                                    ValidaRadios = False
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If

    End Function

    Private Sub btn_Limpiar_Click(sender As Object, e As EventArgs) Handles btn_Limpiar.Click
        Try
            EdoControl(Operacion.Ninguna)
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Limpiar_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Firmar_Click(sender As Object, e As EventArgs) Handles btn_Firmar.Click
        Try
            ' ActualizaDataOP()
            'txtToken.Text = ""


            'fn_Token()
            If fn_Autorizaciones(False) = False Then
                Exit Sub
            End If

            'hid_rechazo.Value = 0
            'Master.Titulo_Autoriza = "AUTORIZACIÓN ORDENES DE PAGO"
            'Funciones.EjecutaFuncion("fn_Autoriza();")
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Firmar_Click: " & ex.Message)
        End Try
    End Sub

    'Private Sub btn_Rechazar_Click(sender As Object, e As EventArgs) Handles btn_Rechazar.Click
    '    Try
    '        'ActualizaDataOP()

    '        If fn_Autorizaciones(True) = False Then
    '            Exit Sub
    '        End If

    '        'hid_rechazo.Value = -1
    '        'Master.Titulo_Autoriza = "RECHAZO ORDENES DE PAGO"
    '        'Funciones.EjecutaFuncion("fn_Autoriza();")
    '    Catch ex As Exception
    '        Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
    '        Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Rechazar_Click: " & ex.Message)
    '    End Try
    'End Sub


    Private Sub btn_Confirmar_Click(sender As Object, e As EventArgs) Handles btn_Confirmar.Click
        'Dim ws As New ws_Generales.GeneralesClient
        Dim ws As New ws_CorreoExterno.GeneralesClient
        Dim strMensaje As String = "FIRMAS: "
        Dim strNroOp As String = vbNullString
        Dim strSalida As String

        Try
            dtCorreos = New DataTable
            dtCorreos.Columns.Add("usuario")
            dtCorreos.Columns.Add("mail_usuario")
            dtCorreos.Columns.Add("subject")
            dtCorreos.Columns.Add("body")
            dtCorreos.Columns.Add("mail_cc")

            If hid_rechazo.Value = -1 Then
                strNroOp = fn_Mails("sn_rechazo = 1 AND sn_firma_rechazo = 0 AND sn_rol_rechazo = True", "", "rechazo", "solicita", Cons.TipoPersona.Solicitante)
                If Not strNroOp = vbNullString Then strMensaje = strMensaje & vbCrLf & "Rechazadas: " & strNroOp
            Else
                'CONTABILIDAD A TESORERIA
                strSalida = fn_Mails("sn_contabilidad = 1 AND sn_firma_contabilidad = 0 AND sn_rol_contabilidad = True", strNroOp, "contabilidad", "tesoreria", Cons.TipoPersona.Tesoreria, 0)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Contabilidad a Tesoreria: " & strSalida

                strSalida = fn_Mails("sn_contabilidad = 1 AND sn_firma_contabilidad = 0 AND sn_rol_contabilidad = True", strNroOp, "contabilidad", "tesoreria", Cons.TipoPersona.Tesoreria, 1)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Contabilidad a Tesoreria (PAGO URGENTE): " & strSalida


                'TESORERIA A CONTABILIDAD
                strSalida = fn_Mails("sn_tesoreria = 1 AND sn_firma_tesoreria = 0 AND sn_rol_tesoreria = True", strNroOp, "tesoreria", "contabilidad", Cons.TipoPersona.Contabilidad, 0)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Tesoreria a Contabilidad: " & strSalida

                strSalida = fn_Mails("sn_tesoreria = 1 AND sn_firma_tesoreria = 0 AND sn_rol_tesoreria = True", strNroOp, "tesoreria", "contabilidad", Cons.TipoPersona.Contabilidad, 1)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Tesoreria a Contabilidad (PAGO URGENTE): " & strSalida


                'DIRECTOR GENERAL A TESORERIA
                strSalida = fn_Mails("sn_director_gral = 1 AND sn_firma_director_gral = 0 AND sn_rol_director_gral = True", strNroOp, "director_gral", "tesoreria", Cons.TipoPersona.Tesoreria, 0)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Director General a Tesoreria: " & strSalida

                strSalida = fn_Mails("sn_director_gral = 1 AND sn_firma_director_gral = 0 AND sn_rol_director_gral = True", strNroOp, "director_gral", "tesoreria", Cons.TipoPersona.Tesoreria, 1)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Director General a Tesoreria (PAGO URGENTE): " & strSalida


                'DIRECTOR A DIRECTOR GENERAL
                strSalida = fn_Mails("sn_director = 1 AND sn_firma_director = 0 AND sn_autoriza_director_gral = 1 AND  sn_rol_director = True", strNroOp, "director", "director_gral", Cons.TipoPersona.DirectorGeneral, 0)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Director a Director General: " & strSalida

                strSalida = fn_Mails("sn_director = 1 AND sn_firma_director = 0 AND sn_autoriza_director_gral = 1 AND  sn_rol_director = True", strNroOp, "director", "director_gral", Cons.TipoPersona.DirectorGeneral, 1)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Director a Director General (PAGO URGENTE): " & strSalida


                'DIRECTOR A TESORERIA
                strSalida = fn_Mails("sn_director = 1 AND sn_firma_director = 0 AND sn_autoriza_director_gral = 0 AND  sn_rol_director = True", strNroOp, "director", "tesoreria", Cons.TipoPersona.Tesoreria, 0)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Director a Tesoreria: " & strSalida

                strSalida = fn_Mails("sn_director = 1 AND sn_firma_director = 0 AND sn_autoriza_director_gral = 0 AND  sn_rol_director = True", strNroOp, "director", "tesoreria", Cons.TipoPersona.Tesoreria, 1)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Director a Tesoreria (PAGO URGENTE): " & strSalida


                'SUBDIRECTOR A DIRECTOR
                strSalida = fn_Mails("sn_subdirector = 1 AND sn_firma_subdirector = 0 AND sn_autoriza_director = 1 AND sn_rol_subdirector = True", strNroOp, "subdirector", "director", Cons.TipoPersona.Director, 0)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Subdirector a Director: " & strSalida

                strSalida = fn_Mails("sn_subdirector = 1 AND sn_firma_subdirector = 0 AND sn_autoriza_director = 1 AND sn_rol_subdirector = True", strNroOp, "subdirector", "director", Cons.TipoPersona.Director, 1)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Subdirector a Director (PAGO URGENTE): " & strSalida


                'SUBDIRECTOR A TESORERIA
                strSalida = fn_Mails("sn_subdirector = 1 AND sn_firma_subdirector = 0 AND sn_autoriza_director = 0 AND sn_rol_subdirector = True", strNroOp, "subdirector", "tesoreria", Cons.TipoPersona.Tesoreria, 0)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Subdirector a Tesoreria: " & strSalida

                strSalida = fn_Mails("sn_subdirector = 1 AND sn_firma_subdirector = 0 AND sn_autoriza_director = 0 AND sn_rol_subdirector = True", strNroOp, "subdirector", "tesoreria", Cons.TipoPersona.Tesoreria, 1)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Subdirector a Tesoreria (PAGO URGENTE): " & strSalida


                'JEFE INMEDIATO A SUBDIRECTOR
                strSalida = fn_Mails("sn_jefe_inmediato = 1 AND sn_firma_jefe = 0 AND sn_rol_jefe = True", strNroOp, "jefe", "subdirector", Cons.TipoPersona.Subdirector, 0)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Jefe Inmediato a Subdirector: " & strSalida

                strSalida = fn_Mails("sn_jefe_inmediato = 1 AND sn_firma_jefe = 0 AND sn_rol_jefe = True", strNroOp, "jefe", "subdirector", Cons.TipoPersona.Subdirector, 1)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Jefe Inmediato a Subdirector (PAGO URGENTE): " & strSalida


                'SOLICITANTE A JEFE INMEDIATO
                strSalida = fn_Mails("sn_solicita = 1 AND sn_firma_solicita = 0 AND sn_rol_solicita = True", strNroOp, "solicita", "jefe", Cons.TipoPersona.JefeInmediato, 0)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Solicitante a Jefe Inmediato: " & strSalida

                strSalida = fn_Mails("sn_solicita = 1 AND sn_firma_solicita = 0 AND sn_rol_solicita = True", strNroOp, "solicita", "jefe", Cons.TipoPersona.JefeInmediato, 1)
                strNroOp = strNroOp & IIf(Len(strNroOp) = 0, "", ",") & If(Len(strSalida) = 0, "-1", strSalida)
                If Not strSalida = vbNullString Then strMensaje = strMensaje & vbCrLf & "Solicitante a Jefe Inmediato (PAGO URGENTE): " & strSalida
            End If

            Dim strNumOrds = CategorizaOPs(hid_rechazo.Value)
            Dim arrNumOrds() As String = strNumOrds.Split("|")

            'UPDATE A FIRMAS
            If Not arrNumOrds(0) = vbNullString Then ActualizaFirmas(arrNumOrds(0), Cons.TipoPersona.Solicitante, 0) 'Solicitante
            If Not arrNumOrds(1) = vbNullString Then ActualizaFirmas(arrNumOrds(1), Cons.TipoPersona.JefeInmediato, 0) 'Jefe Inmediato

            If Not arrNumOrds(2) = vbNullString Then ActualizaFirmas(arrNumOrds(2), Cons.TipoPersona.Subdirector, 0) 'Subdirector via Tesoreria
            If Not arrNumOrds(3) = vbNullString Then ActualizaFirmas(arrNumOrds(3), Cons.TipoPersona.Subdirector, -1) 'Subdirector via Dirección

            If Not arrNumOrds(4) = vbNullString Then ActualizaFirmas(arrNumOrds(4), Cons.TipoPersona.Director, 0) 'Director via Tesoreria
            If Not arrNumOrds(5) = vbNullString Then ActualizaFirmas(arrNumOrds(5), Cons.TipoPersona.Director, -1) 'Director via Director General

            If Not arrNumOrds(6) = vbNullString Then ActualizaFirmas(arrNumOrds(6), Cons.TipoPersona.DirectorGeneral, 0) 'Dirección General

            If Not arrNumOrds(7) = vbNullString Then ActualizaFirmas(arrNumOrds(7), Cons.TipoPersona.Tesoreria, 0) 'Tesoreria
            If Not arrNumOrds(8) = vbNullString Then ActualizaFirmas(arrNumOrds(8), Cons.TipoPersona.Contabilidad, 0) 'Contabilidad
            If Not arrNumOrds(9) = vbNullString Then ActualizaFirmas(arrNumOrds(9), Cons.TipoPersona.Rechazo, 0) 'Rechazos


            'Actualiza el Repositorio
            Funciones.fn_Guarda_OP(strNroOp, Master.user, Eramake.eCryptography.Decrypt(Master.pws), False, False)


            'Envío de Mails
            For Each fila In dtCorreos.Rows
                ws.EnviaCorreo(fila("mail_usuario"), fila("body"), fila("subject"), fila("mail_cc"), "", Master.email, Master.usuario, Eramake.eCryptography.Decrypt(Master.pws),
                               Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario)

            Next


            'Recarga de Grid en tiempo real despues de la Firma
            Funciones.LlenaGrid(grdOrdenPago, dtOrdenPago)
            Funciones.EjecutaFuncion("fn_EstadoFilas('grdOrdenPago',true);")

            'ListaRamosContables()
            DesHabilitaChecksFirma()

            Funciones.CerrarModal("#EsperaModal")

            Mensaje.MuestraMensaje(Master.Titulo, strMensaje, TipoMsg.Confirma)

            Funciones.fn_InsertaBitacora(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "Firmas Electrónicas: " & strMensaje)

        Catch ex As Exception
            Funciones.CerrarModal("#EsperaModal")
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Confirmar_Click: " & ex.Message)
        End Try

    End Sub

    Private Function fn_Autorizaciones(ByVal sn_proceso As Boolean) As Boolean
        Dim strOP As String = ""
        Dim codRol As String = ""
        Dim OPCompletada As Boolean = False

        Dim ws As New ws_Generales.GeneralesClient

        fn_Autorizaciones = False
        dtAutorizaciones = New DataTable
        dtAutorizaciones.Columns.Add("nro_op")
        dtAutorizaciones.Columns.Add("cod_usuario")
        dtAutorizaciones.Columns.Add("user_id")
        dtAutorizaciones.Columns.Add("usuario")

        dtAutorizaciones.Columns.Add("rol")
        dtAutorizaciones.Columns.Add("NivelAutorizacion")
        'Codigos
        dtAutorizaciones.Columns.Add("Solicitante")
        dtAutorizaciones.Columns.Add("Jefe")
        dtAutorizaciones.Columns.Add("Tesoreria")
        dtAutorizaciones.Columns.Add("SubDirector")
        dtAutorizaciones.Columns.Add("Director")
        dtAutorizaciones.Columns.Add("DirectorGeneral")
        dtAutorizaciones.Columns.Add("Subgerente")
        'Switch Banderas
        dtAutorizaciones.Columns.Add("FirmadoSolicitante")
        dtAutorizaciones.Columns.Add("FirmadoJefe")
        dtAutorizaciones.Columns.Add("FirmadoTesoreria")
        dtAutorizaciones.Columns.Add("FirmadoSubdirector")
        dtAutorizaciones.Columns.Add("FirmadoDirector")
        dtAutorizaciones.Columns.Add("FirmadoDirectorGeneral")
        dtAutorizaciones.Columns.Add("FirmadoSubgerente")
        dtAutorizaciones.Columns.Add("NombreModifica")

        For Each row In dtOrdenPago.Rows

            dtAutorizaciones.Rows.Add(row("nro_op"), Master.cod_usuario, Master.user, Master.usuario,
                                      IIf(IsDBNull(row("RoleUsuario")) = vbTrue, "C", row("RoleUsuario")),
                                        row("NivelAutorizacion"),
                                        row("Solicitante"),
                                        row("Jefe"),
                                        row("Tesoreria"),
                                        row("SubDirector"),
                                        row("Director"),
                                        row("DirectorGeneral"),
                                        row("Subgerente"),
                                        row("FirmadoSolicitante"),
                                        row("FirmadoJefe"),
                                        row("FirmadoTesoreria"),
                                        row("FirmadoSubdirector"),
                                        row("FirmadoDirector"),
                                        row("FirmadoDirectorGeneral"),
                                        row("FirmadoSubgerente"),
                                        row("NombreModifica"))
            codRol = IIf(IsDBNull(row("RoleUsuario")) = vbTrue, "C", row("RoleUsuario"))
        Next



        'Nivel 1 Firma analista y supervisor
        'Nivel 2 Firma analista y subgerente
        'Nivel 3 Firma analista y subdirector
        'Nivel 4 Firma analista, subdirector y director
        'Nivel 5 Firma analista, subdirector, director y director general

        Dim UsuarioFirma As String = vbNullString
        Dim contador As Integer = 0
        Dim ResultTran As Integer = 0

        dtAutoriza = New DataTable
        dtAutoriza.Columns.Add("noOP")
        dtCancela = New DataTable
        dtCancela.Columns.Add("noOP")
        dtCancela.Columns.Add("Justificacion")

        Dim numPaginas As Integer
        Dim paginaActual As Integer
        Dim RowsMostrados As Integer

        numPaginas = grdOrdenPago.PageCount
        paginaActual = grdOrdenPago.PageIndex
        RowsMostrados = grdOrdenPago.Rows.Count
        Dim RegTotales As Integer = dtAutorizaciones.Rows.Count

        Dim contadorIni As Integer = 0
        Select Case grdOrdenPago.PageIndex
            Case 0
                contador = 0
                contadorIni = 0
            Case 1
                contador = 0
                contadorIni = 17
            Case 2
                contador = 0
                contadorIni = 37
            Case 3
                contador = 0
                contadorIni = 57
            Case 4
                contador = 0
                contadorIni = 77
        End Select

        For Each row In dtAutorizaciones.Rows


            If contadorIni = dtAutorizaciones.Rows.IndexOf(row) And contador <= (RowsMostrados - 1) Then


                Dim chk_Impresion As Boolean = DirectCast(grdOrdenPago.Rows(contador).FindControl("chkImpresion"), CheckBox).Checked
                Dim chk_Rechazo As Boolean = DirectCast(grdOrdenPago.Rows(contador).FindControl("chk_Rechazo"), CheckBox).Checked
                Dim ddlMotivo = DirectCast(grdOrdenPago.Rows(contador).FindControl("txt_Motivo"), DropDownList)

                Dim txtJustif As String = DirectCast(grdOrdenPago.Rows(contador).FindControl("txt_Motivo"), DropDownList).SelectedItem.Text
                Dim txtOtros As String = DirectCast(grdOrdenPago.Rows(contador).FindControl("txtOtros"), TextBox).Text

                If ddlMotivo.SelectedValue = 11 Then
                    txtJustif = txtOtros
                End If

                strOP = row("nro_op")


                If chk_Impresion = True Then
                    If chk_Rechazo = False Then

                        Dim Rechazada As Integer = fn_Ejecuta("mis_ValidaStsOp " & strOP)
                        If Rechazada = 1 Then
                            Mensaje.MuestraMensaje("Validación", "la Orden de Pago: " & strOP & " ya se encuentra rechazada, por favor deseleccionarla", TipoMsg.Advertencia)
                            Exit Function
                        End If

                        If row("NivelAutorizacion") = 1 Then
                            If DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSolicitante"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Solicitante")
                            ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaJefe"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Jefe")
                            Else
                                fn_Ejecuta("mis_AutorizaOPSTros " & strOP & ",'" & Master.cod_usuario & "'")
                                OPCompletada = True
                            End If

                        ElseIf row("NivelAutorizacion") = 2 Then
                            If DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSolicitante"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Solicitante")
                            ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSubgerente"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Subgerente")
                            Else
                                fn_Ejecuta("mis_AutorizaOPSTros " & strOP & ",'" & Master.cod_usuario & "'")
                                OPCompletada = True
                            End If


                        ElseIf row("NivelAutorizacion") = 3 Then
                            If DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSolicitante"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Solicitante")
                            ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSubdirector"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Subdirector")
                            Else
                                fn_Ejecuta("mis_AutorizaOPSTros " & strOP & ",'" & Master.cod_usuario & "'")
                                OPCompletada = True
                            End If

                        ElseIf row("NivelAutorizacion") = 4 Then
                            If DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSolicitante"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Solicitante")
                            ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSubdirector"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Subdirector")
                            ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaDirector"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Director")
                            Else
                                fn_Ejecuta("mis_AutorizaOPSTros " & strOP & ",'" & Master.cod_usuario & "'")
                                OPCompletada = True
                            End If

                        ElseIf row("NivelAutorizacion") = 5 Then
                            If DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSolicitante"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Solicitante")
                            ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSubdirector"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Subdirector")
                            ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaDirector"), CheckBox).Checked = False Then
                                UsuarioFirma = row("Director")
                            ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaDirectorGeneral"), CheckBox).Checked = False Then
                                UsuarioFirma = row("DirectorGeneral")
                            Else
                                fn_Ejecuta("mis_AutorizaOPSTros " & strOP & ",'" & Master.cod_usuario & "'")
                                OPCompletada = True
                            End If

                        End If
                        If OPCompletada = False Then

                            If sn_proceso = True Then
                                Dim AdminUsuStr As String
                                AdminUsuStr = ws.ObtieneParametro(Cons.TargetFiltrosAdminStro)

                                If InStr(AdminUsuStr, Master.cod_usuario) Then

                                    'If Master.cod_usuario = "CLOPEZ" And Master.cod_usuario = "CREYES" And Master.cod_usuario = "AMEZA" Then
                                    fn_Ejecuta("mis_InsertaOPsEnviadas " & strOP & ",'" & UsuarioFirma & "'," & Cons.StrosTradicional & ",-2")
                                    'fn_Ejecuta("mis_EmailsOPStros '" & strOP & "','" & cmbModuloOP.SelectedItem.Value & "','" & UsuarioFirma & "','" & Master.usuario & "','" & codRol & "'")
                                    Mensaje.MuestraMensaje("Autorizaciones", "Se enviarán las Ordenes de Pago en el horario parametrizado", Mensaje.TipoMsg.Confirma)
                                Else

                                    If fn_Ejecuta("usp_AplicaFirmasOP_stro " & strOP & ",-1,'" & codRol & "'") = 1 Then
                                        fn_Ejecuta("mis_InsertaOPsEnviadas " & strOP & ",'" & UsuarioFirma & "'," & Cons.StrosTradicional & ",0")

                                        fn_Ejecuta("mis_EmailsOPStros '" & strOP & "','" & Cons.StrosTradicional & "','" & UsuarioFirma & "','" & Master.usuario & "','" & codRol & "'")
                                        Mensaje.MuestraMensaje("Autorizaciones", "Se aplicaron las firmas correspondientes", Mensaje.TipoMsg.Confirma)
                                    End If
                                End If
                            End If
                        End If
                        dtAutoriza.Rows.Add(strOP)

                    Else 'rechazo
                        Dim codMotivoRechazo As Integer
                        Dim strMotivoRechazo As String
                        Dim intFolioOnBase As Integer
                        codMotivoRechazo = DirectCast(grdOrdenPago.Rows(contador).FindControl("txt_Motivo"), DropDownList).SelectedItem.Value
                        strMotivoRechazo = DirectCast(grdOrdenPago.Rows(contador).FindControl("txt_Motivo"), DropDownList).SelectedItem.Text
                        intFolioOnBase = DirectCast(grdOrdenPago.Rows(contador).FindControl("lblFolioOnBase"), Label).Text

                        Dim Rechazada As Integer = fn_Ejecuta("mis_ValidaStsOp " & strOP)
                        If Rechazada = 1 Then
                            Mensaje.MuestraMensaje("Validación Rechazos", "la Orden de Pago: " & strOP & " ya se encuentra rechazada, por favor deseleccionarla", TipoMsg.Advertencia)
                            Exit Function
                        End If

                        If strMotivoRechazo = "--Seleccione--" Then
                            Mensaje.MuestraMensaje("Validación Motivo de Rechazo", "No se ha seleccionado ningun motivo de rechazo de para la OP: " & strOP, TipoMsg.Advertencia)
                            Exit Function
                        End If

                        If ddlMotivo.SelectedValue = 11 Then
                            strMotivoRechazo = txtOtros

                        End If

                        If sn_proceso = True Then


                            If row("NivelAutorizacion") = 1 Then

                                If DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaJefe"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Jefe")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")

                                ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSolicitante"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Solicitante")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                End If

                            ElseIf row("NivelAutorizacion") = 2 Then

                                If DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSubgerente"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Subgerente")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSolicitante"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Solicitante")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                End If

                            ElseIf row("NivelAutorizacion") = 3 Then

                                If DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSubdirector"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Subdirector")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSolicitante"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Solicitante")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                End If

                            ElseIf row("NivelAutorizacion") = 4 Then
                                If DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaDirector"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Director")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSubdirector"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Subdirector")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSolicitante"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Solicitante")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                End If

                            ElseIf row("NivelAutorizacion") = 5 Then

                                If DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaDirectorGeneral"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("DirectorGeneral")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaDirector"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Director")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSubdirector"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Subdirector")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmaSolicitante"), CheckBox).Checked = True Then
                                    UsuarioFirma = row("Solicitante")
                                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "'")
                                End If

                            End If

                            'Aqui corria la cancelacion. se comenta por 2da fase
                            fn_Ejecuta("usp_AplicaFirmasOP_stro " & strOP & ",0,'" & codRol & "','Usuario: " & Master.usuario & " /Motivo: " & strMotivoRechazo & "','" & Master.cod_usuario & "'")
                            'fn_Ejecuta("mis_CancelaOPStros " & strOP & ",'" & Master.cod_usuario & "'," & codMotivoRechazo & "," & intFolioOnBase)

                            'fn_Ejecuta("mis_MailOpRechazo '" & strOP & "','CLOPEZ','" & Master.usuario & "'")
                            'fn_Ejecuta("mis_MailOpRechazo '" & strOP & "','" & row("NombreModifica") & "','" & Master.usuario & "'")

                            fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','CLOPEZ','" & Master.usuario & "'")
                            fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & row("NombreModifica") & "','" & Master.usuario & "'")
                        Else
                            dtCancela.Rows.Add(strOP, txtJustif)
                        End If

                    End If

                End If
                contador = contador + 1
                contadorIni = contadorIni + 1
            Else
                'contador = contador + 1
            End If


        Next

        gvd_Canceladas.DataSource = dtCancela
        gvd_Canceladas.DataBind()
        gvd_Autorizadas.DataSource = dtAutoriza
        gvd_Autorizadas.DataBind()

        If dtCancela Is Nothing And dtAutoriza Is Nothing Then
            Mensaje.MuestraMensaje(Master.Titulo, "No se ha seleccionado ninguna Orden de Pago para rechazar ó autorizar", TipoMsg.Advertencia)
        Else

            gvd_Canceladas.DataSource = dtCancela
            gvd_Canceladas.DataBind()
            gvd_Autorizadas.DataSource = dtAutoriza
            gvd_Autorizadas.DataBind()

            If sn_proceso = True Then
                If Master.cod_usuario <> "CLOPEZ" Then

                    Funciones.fn_Consulta("mis_ObtieneOpEnvioFirma 0,''," & Cons.StrosTradicional, dtEnvios)

                    For Each item In dtEnvios.Rows
                        fn_Ejecuta("mis_EmailsOPStros '" & item("Ops") & "','" & item("tipomodulo") & "','" & item("cod_usuario") & "','" & Master.usuario & "','" & codRol & "'")
                        fn_Ejecuta("mis_ActualizaStsOpsEnv '" & item("Ops") & "','" & item("cod_usuario") & "'," & Cons.StrosTradicional & ",1")
                    Next
                    fn_Ejecuta("mis_EmailsOPStros '" & strOP & "','" & Cons.StrosTradicional & "','" & UsuarioFirma & "','" & Master.usuario & "','" & codRol & "'")
                    Mensaje.MuestraMensaje("Autorizaciones", "Se realizaron las acciones correctamente", Mensaje.TipoMsg.Confirma)

                End If

                btn_Limpiar_Click(Nothing, Nothing)

            End If
        End If
        fn_Autorizaciones = True

        If sn_proceso = False Then
            If gvd_Canceladas.Rows.Count > 0 Or gvd_Autorizadas.Rows.Count > 0 Then
                txtToken.Text = ""
                fn_Token()
            End If
            Funciones.AbrirModal("#Resumen")
            Else
                txtToken.Text = ""
            Mensaje.MuestraMensaje("Proceso Autorizacion Electrónica", "Se realizaron las acciones correctamente", TipoMsg.Confirma)
            Funciones.CerrarModal("#Resumen")
        End If
        '    Else
        '        Mensaje.MuestraMensaje(Master.Titulo, "No se ha seleccionado ninguna Orden de Pago para " & IIf(sn_rechazo = True, "rechazar", "autorizar"), TipoMsg.Falla)
        'End If

    End Function
    Private Function fn_ObtieneMail(cod_usuario As String) As String
        Dim eMail As String = ""
        Try
            Funciones.fn_Consulta("mis_ConsultaMail '" & Master.cod_usuario & "'", dtCorreos)
            If dtCorreos.Rows.Count > 0 Then eMail = dtCorreos(0).ItemArray(0).ToString

        Catch ex As Exception

        End Try

        Return eMail
    End Function

    Private Sub fn_Token()
        Try
            Funciones.fn_Consulta("mis_InsertaToken '" & Master.cod_usuario & "'", dtToken)
            If dtToken.Rows.Count > 0 Then hid_Token.Value = dtToken(0).ItemArray(0).ToString

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, String.Format("Token Error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub

    Private Function fn_Mails(ByVal Consulta As String, str_enviados As String, ByVal solicita As String, ByVal envia As String, cod_rol As Integer, Optional ByVal sn_urgente As Integer = 0) As String
        Dim ws As New ws_Generales.GeneralesClient
        Dim strOp As String = vbNullString
        Dim strRemitente As String = vbNullString
        Dim strMotivo As String = vbNullString
        Dim strBody As String = vbNullString
        Dim strSubject As String = vbNullString
        Dim strCC As String
        Dim RowSql() As Data.DataRow
        Dim dtDatos As DataTable
        fn_Mails = vbNullString

        'Consulta de acuerdo al tipo de autorización
        RowSql = dtOrdenPago.Select(Consulta & " AND nro_op NOT IN (" & IIf(Len(str_enviados) = 0, "-1", str_enviados) & ") AND sn_urgente = " & sn_urgente)

        If RowSql.Count > 0 Then
            dtDatos = RowSql.CopyToDataTable()

            'Obtiene los distintos destinatarios
            Dim query = From row In dtDatos.AsEnumerable()
                        Group row By usuario = row("usuario_" & envia),
                                     mail = row("mail_" & envia)
                            Into users = Group
                        Select New With {
                                            Key usuario,
                                            Key mail
                                        }


            For Each fila In query
                strCC = ""
                strOp = vbNullString
                strRemitente = vbNullString
                strMotivo = vbNullString

                Dim qryOp = From q In (From p In dtDatos.AsEnumerable()
                                       Where p("usuario_" & envia) = fila.usuario
                                       Select New With {.nro_op = p("nro_op"),
                                                        .usuario = p("usuario_" & solicita),
                                                        .motivo_rechazo = p("motivo_rechazo")})
                            Select q.nro_op, q.usuario, q.motivo_rechazo

                For Each filaOp In qryOp
                    strOp = strOp & filaOp.nro_op & ","
                    If Not InStr(strRemitente, filaOp.usuario) > 0 Then
                        strRemitente = strRemitente & filaOp.usuario & " " & vbCrLf
                    End If
                    strMotivo = strMotivo & filaOp.motivo_rechazo & " " & vbCrLf
                Next

                If Len(strOp) > 0 Then strOp = Left(strOp, Len(strOp) - 1)

                If solicita = "rechazo" Then
                    strSubject = "PAGOS REASEGURO FACULTATIVO (RECHAZO)"
                    strBody = Funciones.FormatoCorreo(TipoFormato.Rechazo, strOp, strRemitente, cod_rol, strMotivo, 0)
                ElseIf solicita = "contabilidad" Then
                    strSubject = "PAGOS REASEGURO FACULTATIVO (CONFIRMACIÓN) " & IIf(sn_urgente = 1, "<----PAGO URGENTE---->", "")
                    strBody = Funciones.FormatoCorreo(TipoFormato.VistoBueno, strOp, strRemitente, cod_rol, "", sn_urgente)

                    Dim myrow() As Data.DataRow
                    myrow = dtOrdenPago.Select("nro_op IN (" & strOp & ")")

                    If myrow.Count > 0 Then
                        dtConsulta = myrow.CopyToDataTable()

                        'Obtiene los distintos destinatarios solicitantes
                        Dim query_b = From row In dtConsulta.AsEnumerable()
                                      Group row By mail = row("mail_solicita")
                                      Into users = Group
                                      Select New With {
                                                        Key mail
                                                      }

                        For Each item In query_b
                            strCC = strCC & item.mail & ","
                        Next

                        If Len(strCC) > 0 Then strCC = Left(strCC, Len(strCC) - 1)
                    End If

                Else
                    strSubject = "PAGOS REASEGURO FACULTATIVO " & IIf(sn_urgente = 1, "<----PAGO URGENTE---->", "")
                    strBody = Funciones.FormatoCorreo(TipoFormato.Autorizacion, strOp, strRemitente, cod_rol, "", sn_urgente)
                End If

                dtCorreos.Rows.Add(fila.usuario, fila.mail, strSubject, strBody, strCC)

                fn_Mails = fn_Mails & strOp & ","
            Next
        End If

        If Len(fn_Mails) > 0 Then fn_Mails = Left(fn_Mails, Len(fn_Mails) - 1)




    End Function

    Private Function ActualizaDataOP() As DataTable

        For Each row In grdOrdenPago.Rows
            Dim chk_SelOp = DirectCast(row.FindControl("chk_SelOp"), CheckBox)
            Dim txt_Estado = DirectCast(row.FindControl("txt_Estado"), TextBox)
            Dim chk_Impresion = DirectCast(row.FindControl("chk_Impresion"), CheckBox)
            Dim chk_Manual = DirectCast(row.FindControl("chk_Manual"), CheckBox)
            Dim chk_FirmaSol = DirectCast(row.FindControl("chk_FirmaSol"), CheckBox)
            Dim chk_FirmaJefe = DirectCast(row.FindControl("chk_FirmaJefe"), CheckBox)
            Dim chk_SubDir = DirectCast(row.FindControl("chk_SubDir"), CheckBox)
            Dim chk_FirmaDir = DirectCast(row.FindControl("chk_FirmaDir"), CheckBox)
            Dim chk_AutorizaDireccion = DirectCast(row.FindControl("chk_AutorizaDireccion"), CheckBox)
            Dim chk_FirmaDirGral = DirectCast(row.FindControl("chk_FirmaDirGral"), CheckBox)
            Dim chk_AutorizaDirectorGral = DirectCast(row.FindControl("chk_AutorizaDirectorGral"), CheckBox)
            Dim chk_FirmaTeso = DirectCast(row.FindControl("chk_FirmaTeso"), CheckBox)
            Dim chk_FirmaCon = DirectCast(row.FindControl("chk_FirmaCon"), CheckBox)
            Dim chk_Rechazo = DirectCast(row.FindControl("chk_Rechazo"), CheckBox)
            Dim txt_Motivo = DirectCast(row.FindControl("txt_Motivo"), TextBox)
            Dim chk_Urgente = DirectCast(row.FindControl("chk_Urgente"), CheckBox)
            Dim chk_Financiado = DirectCast(row.FindControl("chk_Financiado"), CheckBox)

            Dim img_FirmaSolicita = DirectCast(row.FindControl("img_FirmaSolicita"), Image)
            Dim lnk_SelSolicitante = DirectCast(row.FindControl("lnk_SelSolicitante"), LinkButton)
            Dim hid_SelSolicitante = DirectCast(row.FindControl("hid_SelSolicitante"), HiddenField)
            Dim hid_CodSolicitante = DirectCast(row.FindControl("hid_CodSolicitante"), HiddenField)
            Dim hid_MailSolicitante = DirectCast(row.FindControl("hid_MailSolicitante"), HiddenField)

            Dim img_FirmaJefe = DirectCast(row.FindControl("img_FirmaJefe"), Image)
            Dim lnk_SelJefe = DirectCast(row.FindControl("lnk_SelJefe"), LinkButton)
            Dim hid_SelJefe = DirectCast(row.FindControl("hid_SelJefe"), HiddenField)
            Dim hid_CodJefe = DirectCast(row.FindControl("hid_CodJefe"), HiddenField)
            Dim hid_MailJefe = DirectCast(row.FindControl("hid_MailJefe"), HiddenField)

            Dim img_FirmaSubditector = DirectCast(row.FindControl("img_FirmaSubditector"), Image)
            Dim lnk_SelSubDir = DirectCast(row.FindControl("lnk_SelSubDir"), LinkButton)
            Dim hid_SelSubDir = DirectCast(row.FindControl("hid_SelSubDir"), HiddenField)
            Dim hid_CodSubDir = DirectCast(row.FindControl("hid_CodSubDir"), HiddenField)
            Dim hid_MailSubDir = DirectCast(row.FindControl("hid_MailSubDir"), HiddenField)

            Dim img_FirmaDirector = DirectCast(row.FindControl("img_FirmaDirector"), Image)
            Dim lnk_SelDir = DirectCast(row.FindControl("lnk_SelDir"), LinkButton)
            Dim hid_SelDir = DirectCast(row.FindControl("hid_SelDir"), HiddenField)
            Dim hid_CodDir = DirectCast(row.FindControl("hid_CodDir"), HiddenField)
            Dim hid_MailDir = DirectCast(row.FindControl("hid_MailDir"), HiddenField)

            Dim img_FirmaDirectorGral = DirectCast(row.FindControl("img_FirmaDirectorGral"), Image)
            Dim lnk_SelDirGral = DirectCast(row.FindControl("lnk_SelDirGral"), LinkButton)
            Dim hid_SelDirGral = DirectCast(row.FindControl("hid_SelDirGral"), HiddenField)
            Dim hid_CodDirGral = DirectCast(row.FindControl("hid_CodDirGral"), HiddenField)
            Dim hid_MailDirGral = DirectCast(row.FindControl("hid_MailDirGral"), HiddenField)

            Dim img_FirmaTesoreria = DirectCast(row.FindControl("img_FirmaTesoreria"), Image)
            Dim lnk_SelTeso = DirectCast(row.FindControl("lnk_SelTeso"), LinkButton)
            Dim hid_SelTeso = DirectCast(row.FindControl("hid_SelTeso"), HiddenField)
            Dim hid_CodTeso = DirectCast(row.FindControl("hid_CodTeso"), HiddenField)
            Dim hid_MailTeso = DirectCast(row.FindControl("hid_MailTeso"), HiddenField)

            Dim img_FirmaContabilidad = DirectCast(row.FindControl("img_FirmaContabilidad"), Image)
            Dim lnk_SelConta = DirectCast(row.FindControl("lnk_SelConta"), LinkButton)
            Dim hid_SelConta = DirectCast(row.FindControl("hid_SelConta"), HiddenField)
            Dim hid_CodConta = DirectCast(row.FindControl("hid_CodConta"), HiddenField)
            Dim hid_MailConta = DirectCast(row.FindControl("hid_MailConta"), HiddenField)

            Dim lbl_OrdenPago = DirectCast(row.FindControl("lbl_OrdenPago"), Label)
            Dim myRow() As Data.DataRow
            myRow = dtOrdenPago.Select("nro_op ='" & lbl_OrdenPago.Text & "'")

            myRow(0)("tSEl_Val") = chk_SelOp.Checked
            myRow(0)("sn_impresion") = chk_Impresion.Checked
            myRow(0)("sn_manual") = IIf(chk_Manual.Checked = True, 1, 0)
            myRow(0)("sn_solicita") = IIf(chk_FirmaSol.Checked = True, 1, 0)
            myRow(0)("sn_jefe_inmediato") = IIf(chk_FirmaJefe.Checked = True, 1, 0)
            myRow(0)("sn_subdirector") = IIf(chk_SubDir.Checked = True, 1, 0)
            myRow(0)("sn_autoriza_director") = IIf(chk_AutorizaDireccion.Checked = True, 1, 0)
            myRow(0)("sn_director") = IIf(chk_FirmaDir.Checked = True, 1, 0)
            myRow(0)("sn_autoriza_director_gral") = IIf(chk_AutorizaDirectorGral.Checked = True, 1, 0)
            myRow(0)("sn_director_gral") = IIf(chk_FirmaDirGral.Checked = True, 1, 0)
            myRow(0)("sn_tesoreria") = IIf(chk_FirmaTeso.Checked = True, 1, 0)
            myRow(0)("sn_contabilidad") = IIf(chk_FirmaCon.Checked = True, 1, 0)
            myRow(0)("sn_rechazo") = IIf(chk_Rechazo.Checked = True, 1, 0)
            myRow(0)("motivo_rechazo") = txt_Motivo.Text
            myRow(0)("sn_urgente") = IIf(chk_Urgente.Checked = True, 1, 0)
            myRow(0)("sn_financiado") = IIf(chk_Financiado.Checked = True, 1, 0)

            myRow(0)("cod_usuario_solicita") = hid_CodSolicitante.Value


            myRow(0)("firma_solicita") = Convert.FromBase64String(img_FirmaSolicita.ImageUrl.Replace("data:image/png;base64,", String.Empty))
            myRow(0)("usuario_solicita") = lnk_SelSolicitante.Text
            myRow(0)("user_id_solicita") = hid_SelSolicitante.Value
            myRow(0)("mail_solicita") = hid_MailSolicitante.Value


            myRow(0)("cod_usuario_jefe") = hid_CodJefe.Value
            myRow(0)("firma_jefe") = Convert.FromBase64String(img_FirmaJefe.ImageUrl.Replace("data:image/png;base64,", String.Empty))
            myRow(0)("usuario_jefe") = lnk_SelJefe.Text
            myRow(0)("user_id_jefe") = hid_SelJefe.Value
            myRow(0)("mail_jefe") = hid_MailJefe.Value

            myRow(0)("cod_usuario_subdirector") = hid_CodSubDir.Value
            myRow(0)("firma_subdirector") = Convert.FromBase64String(img_FirmaSubditector.ImageUrl.Replace("data:image/png;base64,", String.Empty))
            myRow(0)("usuario_subdirector") = lnk_SelSubDir.Text
            myRow(0)("user_id_subdirector") = hid_SelSubDir.Value
            myRow(0)("mail_subdirector") = hid_MailSubDir.Value

            myRow(0)("cod_usuario_director") = hid_CodDir.Value
            myRow(0)("firma_director") = Convert.FromBase64String(img_FirmaDirector.ImageUrl.Replace("data:image/png;base64,", String.Empty))
            myRow(0)("usuario_director") = lnk_SelDir.Text
            myRow(0)("user_id_director") = hid_SelDir.Value
            myRow(0)("mail_director") = hid_MailDir.Value

            myRow(0)("cod_usuario_director_gral") = hid_CodDirGral.Value
            myRow(0)("firma_director_gral") = Convert.FromBase64String(img_FirmaDirectorGral.ImageUrl.Replace("data:image/png;base64,", String.Empty))
            myRow(0)("usuario_director_gral") = lnk_SelDirGral.Text
            myRow(0)("user_id_director_gral") = hid_SelDirGral.Value
            myRow(0)("mail_director_gral") = hid_MailDirGral.Value

            myRow(0)("cod_usuario_tesoreria") = hid_CodTeso.Value
            myRow(0)("firma_tesoreria") = Convert.FromBase64String(img_FirmaTesoreria.ImageUrl.Replace("data:image/png;base64,", String.Empty))
            myRow(0)("usuario_tesoreria") = lnk_SelTeso.Text
            myRow(0)("user_id_tesoreria") = hid_SelTeso.Value
            myRow(0)("mail_tesoreria") = hid_MailTeso.Value

            myRow(0)("cod_usuario_contabilidad") = hid_CodConta.Value
            myRow(0)("firma_contabilidad") = Convert.FromBase64String(img_FirmaContabilidad.ImageUrl.Replace("data:image/png;base64,", String.Empty))
            myRow(0)("usuario_contabilidad") = lnk_SelConta.Text
            myRow(0)("user_id_contabilidad") = hid_SelConta.Value
            myRow(0)("mail_contabilidad") = hid_MailConta.Value

            myRow(0)("sn_ocultar") = txt_Estado.Text
        Next
        Return dtOrdenPago
    End Function

    Private Function CategorizaOPs(ByVal sn_rechazo As Integer) As String
        Dim strOPRechazo As String = ""
        Dim strOPConta As String = ""
        Dim strOPTeso As String = ""
        Dim strOPJefe As String = ""
        Dim strOPDir As String = ""
        Dim strOPDir2 As String = ""
        Dim strOPDirGral As String = ""
        Dim strOPSubDir2 As String = ""
        Dim strOPSubDir1 As String = ""
        Dim strOPSol As String = ""
        Dim strFinal As String = ""
        Dim cod_usuario_sii As String = ""


        'Evaluación de Firmas de Falso a Verdadero
        For Each row In dtOrdenPago.Rows
            If sn_rechazo = -1 Then
                If row("sn_rechazo") = 1 And row("sn_firma_rechazo") = 0 And row("sn_rol_rechazo") = True Then
                    strOPRechazo = strOPRechazo & "(" & row("nro_op") & ",''" & Master.cod_usuario & "'',''" & row("motivo_rechazo") & "'',''" & row("motivo_rechazo") & "''),"
                End If
            Else
                If row("sn_contabilidad") = 1 And row("sn_firma_contabilidad") = 0 And row("sn_rol_contabilidad") = True Then
                    strOPConta = strOPConta & "(" & row("nro_op") & ",''" & row("cod_usuario_contabilidad") & "'',NULL,NULL),"
                End If

                If row("sn_tesoreria") = 1 And row("sn_firma_tesoreria") = 0 And row("sn_rol_tesoreria") = True Then
                    strOPTeso = strOPTeso & "(" & row("nro_op") & ",''" & row("cod_usuario_tesoreria") & "'',''" & row("cod_usuario_contabilidad") & "'',NULL),"
                End If

                If row("sn_director_gral") = 1 And row("sn_firma_director_gral") = 0 And row("sn_rol_director_gral") = True Then
                    strOPDirGral = strOPDirGral & "(" & row("nro_op") & ",''" & row("cod_usuario_director_gral") & "'',''" & row("cod_usuario_tesoreria") & "'',NULL),"
                End If

                If row("sn_autoriza_director_gral") = 1 Then
                    If row("sn_director") = 1 And row("sn_firma_director") = 0 And row("sn_rol_director") = True Then
                        strOPDir2 = strOPDir2 & "(" & row("nro_op") & ",''" & row("cod_usuario_director") & "'',''" & row("cod_usuario_director_gral") & "'',NULL),"
                    End If
                Else
                    If row("sn_director") = 1 And row("sn_firma_director") = 0 And row("sn_rol_director") = True Then
                        strOPDir = strOPDir & "(" & row("nro_op") & ",''" & row("cod_usuario_director") & "'',''" & row("cod_usuario_tesoreria") & "'',NULL),"
                    End If
                End If

                If row("sn_autoriza_director") = 1 Then
                    If row("sn_subdirector") = 1 And row("sn_firma_subdirector") = 0 And row("sn_rol_subdirector") = True Then
                        strOPSubDir2 = strOPSubDir2 & "(" & row("nro_op") & ",''" & row("cod_usuario_subdirector") & "'',''" & row("cod_usuario_director") & "'',NULL),"
                    End If
                Else
                    If row("sn_subdirector") = 1 And row("sn_firma_subdirector") = 0 And row("sn_rol_subdirector") = True Then
                        strOPSubDir1 = strOPSubDir1 & "(" & row("nro_op") & ",''" & row("cod_usuario_subdirector") & "'',''" & row("cod_usuario_tesoreria") & "'',NULL),"
                    End If
                End If

                If row("sn_jefe_inmediato") = 1 And row("sn_firma_jefe") = 0 And row("sn_rol_jefe") = True Then
                    strOPJefe = strOPJefe & "(" & row("nro_op") & ",''" & row("cod_usuario_jefe") & "'',''" & row("cod_usuario_subdirector") & "'',NULL),"
                End If

                If row("sn_solicita") = 1 And row("sn_firma_solicita") = 0 And row("sn_rol_solicita") = True Then
                    strOPSol = strOPSol & "(" & row("nro_op") & ",''" & row("cod_usuario_solicita") & "'',''" & row("cod_usuario_jefe") & "'',NULL),"
                End If
            End If
        Next

        If Len(strOPSol) > 0 Then strOPSol = Left(strOPSol, Len(strOPSol) - 1)
        If Len(strOPJefe) > 0 Then strOPJefe = Left(strOPJefe, Len(strOPJefe) - 1)
        If Len(strOPSubDir1) > 0 Then strOPSubDir1 = Left(strOPSubDir1, Len(strOPSubDir1) - 1) 'Via Tesoreria
        If Len(strOPSubDir2) > 0 Then strOPSubDir2 = Left(strOPSubDir2, Len(strOPSubDir2) - 1) 'Via Dirección
        If Len(strOPDir) > 0 Then strOPDir = Left(strOPDir, Len(strOPDir) - 1) 'Via Tesoreria
        If Len(strOPDir2) > 0 Then strOPDir2 = Left(strOPDir2, Len(strOPDir2) - 1) 'Via Director General
        If Len(strOPDirGral) > 0 Then strOPDirGral = Left(strOPDirGral, Len(strOPDirGral) - 1)
        If Len(strOPTeso) > 0 Then strOPTeso = Left(strOPTeso, Len(strOPTeso) - 1)
        If Len(strOPConta) > 0 Then strOPConta = Left(strOPConta, Len(strOPConta) - 1)
        If Len(strOPRechazo) > 0 Then strOPRechazo = Left(strOPRechazo, Len(strOPRechazo) - 1)

        strFinal = strOPSol & "|" & strOPJefe & "|" & strOPSubDir1 & "|" & strOPSubDir2 & "|" & strOPDir & "|" & strOPDir2 & "|" & strOPDirGral & "|" & strOPTeso & "|" & strOPConta & "|" & strOPRechazo

        Return strFinal

    End Function

    Private Sub ActualizaFirmas(NumOp As String, cod_rol As Integer, sn_autoriza_director As Integer)
        Dim strOrden() As String
        Dim myRow() As Data.DataRow
        If fn_Ejecuta("spU_AplicaFirmasOP '" & NumOp & "'," & cod_rol & "," & sn_autoriza_director) = 1 Then
            strOrden = Split(Replace(Replace(NumOp, ",(", ""), "(", ""), ")")

            For Each nro_op In strOrden
                If Len(nro_op) > 0 Then
                    myRow = dtOrdenPago.Select("nro_op ='" & Split(nro_op, ",")(0) & "'")
                    Select Case cod_rol
                        Case Cons.TipoPersona.Solicitante
                            myRow(0)("sn_firma_solicita") = 1
                        Case Cons.TipoPersona.JefeInmediato
                            myRow(0)("sn_firma_jefe") = 1
                        Case Cons.TipoPersona.Subdirector
                            myRow(0)("sn_firma_subdirector") = 1
                        Case Cons.TipoPersona.Director
                            myRow(0)("sn_firma_director") = 1
                        Case Cons.TipoPersona.DirectorGeneral
                            myRow(0)("sn_firma_director_gral") = 1
                        Case Cons.TipoPersona.Tesoreria
                            myRow(0)("sn_firma_tesoreria") = 1
                        Case Cons.TipoPersona.Contabilidad
                            myRow(0)("sn_firma_contabilidad") = 1
                        Case Cons.TipoPersona.Rechazo
                            myRow(0)("sn_firma_rechazo") = 1
                    End Select
                End If
            Next
        End If


    End Sub

    'Private Sub ListaRamosContables()
    '    Dim Ramos() As String

    '    For Each Row In grdOrdenPago.Rows
    '        Dim lbl_RamosContables As Label = Row.FindControl("lbl_RamosContables")
    '        Dim ddl_RamosContables As DropDownList = Row.FindControl("ddl_RamosContables")

    '        Ramos = Split(lbl_RamosContables.Text, "|")
    '        For intPos = 1 To UBound(Ramos)
    '            ddl_RamosContables.Items.Add(Ramos(intPos))
    '        Next
    '        ddl_RamosContables.SelectedIndex = 0
    '    Next
    'End Sub


    Protected Sub chkFirmaSolicitante_CheckedChanged(sender As Object, e As EventArgs)
        Try

            Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, CheckBox).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)

            If Not sender.checked Then

                Select Case CInt(grdOrdenPago.DataKeys(gr.RowIndex)("NivelAutorizacion"))

                    Case 1

                        'Se retira firma del supervisor
                        TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaJefe"), CheckBox).Checked = False
                        chkFirmaJefe_CheckedChanged(TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaJefe"), CheckBox), Nothing)

                    Case 2

                        'Se retira firma del subgerente
                        TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubgerente"), CheckBox).Checked = False
                        chkFirmaSubgerente_CheckedChanged(TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubgerente"), CheckBox), Nothing)

                    Case 3, 4, 5

                        'Se retira firma del subdirector
                        TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubdirector"), CheckBox).Checked = False
                        chkFirmaSubdirector_CheckedChanged(TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubdirector"), CheckBox), Nothing)

                End Select

                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaSolicitante"), Label).Visible = True
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = False

            Else
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaSolicitante"), Label).Visible = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = True
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chk_Rechazo"), CheckBox).Checked = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).Visible = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).SelectedIndex = 0
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lnk_SelMotivo"), Label).Visible = False
            End If


            If Not fn_Ejecuta(String.Format("usp_ActualizarFirmasOP_stro {0}, {1}, {2} ", grdOrdenPago.DataKeys(gr.RowIndex)("nro_op"), "S", IIf(sender.checked, -1, 0))) = 1 Then
                Mensaje.MuestraMensaje(Master.Titulo, "No se ha podido actualizar la firma del usuario solicitante.", TipoMsg.Falla)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox).Checked = False
                'TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = False

                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaSolicitante"), Label).Visible = True
            Else

                'TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = True
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, String.Format("chkFirmaSolicitante_CheckedChanged Error: {0}", ex.Message), TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, String.Format("chkFirmaSolicitante_CheckedChanged: {0}", ex.Message))
        End Try
    End Sub

    Protected Sub chkFirmaJefe_CheckedChanged(sender As Object, e As EventArgs)

        Try

            Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, CheckBox).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)

            If Not sender.checked Then

                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaJefe"), Label).Visible = True

                'Se retiran las firmas
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox).Checked = False

                'Se dispara evento para retirar firmas
                chkFirmaTesoreria_CheckedChanged(TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox), Nothing)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = False
            Else

                'Si no existen las firmas necesarias que anteceden a esta no se puede firmar
                If Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox).Checked Then

                    Mensaje.MuestraMensaje(Master.Titulo, "Se debe de contar con la firma del solicitante. ", TipoMsg.Falla)
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaJefe"), CheckBox).Checked = False

                Else
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaJefe"), Label).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = True
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chk_Rechazo"), CheckBox).Checked = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).SelectedIndex = 0
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lnk_SelMotivo"), Label).Visible = False
                End If

            End If

            If Not fn_Ejecuta(String.Format("usp_ActualizarFirmasOP_stro {0}, {1}, {2} ", grdOrdenPago.DataKeys(gr.RowIndex)("nro_op"), "J", IIf(sender.checked, -1, 0))) = 1 Then
                Mensaje.MuestraMensaje(Master.Titulo, "No se ha podido actualizar la firma del supervisor.", TipoMsg.Falla)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaJefe"), CheckBox).Checked = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaJefe"), Label).Visible = True
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, String.Format("chkFirmaJefe_CheckedChanged Error: {0}", ex.Message), TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, String.Format("chkFirmaJefe_CheckedChanged: {0}", ex.Message))
        End Try

    End Sub

    Protected Sub chkFirmaSubgerente_CheckedChanged(sender As Object, e As EventArgs)

        Try

            Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, CheckBox).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)

            If Not sender.checked Then

                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaSubgerente"), Label).Visible = True

                'Se retiran las firmas
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox).Checked = False

                'Se dispara evento para retirar firmas
                chkFirmaTesoreria_CheckedChanged(TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox), Nothing)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = False
            Else

                'Si no existen las firmas necesarias que anteceden a esta no se puede firmar
                If Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox).Checked Then

                    Mensaje.MuestraMensaje(Master.Titulo, "Se debe de contar con la firma del solicitante. ", TipoMsg.Falla)
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubgerente"), CheckBox).Checked = False

                Else
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaSubgerente"), Label).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = True
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chk_Rechazo"), CheckBox).Checked = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).SelectedIndex = 0
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lnk_SelMotivo"), Label).Visible = False
                End If

            End If

            If Not fn_Ejecuta(String.Format("usp_ActualizarFirmasOP_stro {0}, {1}, {2} ", grdOrdenPago.DataKeys(gr.RowIndex)("nro_op"), "SG", IIf(sender.checked, -1, 0))) = 1 Then
                Mensaje.MuestraMensaje(Master.Titulo, "No se ha podido actualizar la firma del subgerente.", TipoMsg.Falla)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubgerente"), CheckBox).Checked = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaSubgerente"), Label).Visible = True
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, String.Format("chkFirmaSubgerente_CheckedChanged Error: {0}", ex.Message), TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, String.Format("chkFirmaSubgerente_CheckedChanged: {0}", ex.Message))
        End Try

    End Sub

    Protected Sub chkFirmaSubdirector_CheckedChanged(sender As Object, e As EventArgs)

        Try

            Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, CheckBox).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)

            If Not sender.checked Then

                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaSubdirector"), Label).Visible = True


                Select Case CInt(grdOrdenPago.DataKeys(gr.RowIndex)("NivelAutorizacion"))

                    Case 3

                        'Se retira firma de tesorería
                        TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox).Checked = False
                        chkFirmaTesoreria_CheckedChanged(TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox), Nothing)

                    Case 4, 5

                        'Se retira firma de director
                        TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirector"), CheckBox).Checked = False
                        chkFirmaDirector_CheckedChanged(TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirector"), CheckBox), Nothing)

                End Select
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = False
            Else

                'Si no existen las firmas necesarias que anteceden a esta no se puede firmar
                If Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox).Checked Then

                    Mensaje.MuestraMensaje(Master.Titulo, "Se debe de contar con la firma del solicitante. ", TipoMsg.Falla)
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubdirector"), CheckBox).Checked = False

                Else
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaSubdirector"), Label).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = True
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chk_Rechazo"), CheckBox).Checked = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).SelectedIndex = 0
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lnk_SelMotivo"), Label).Visible = False
                End If

            End If

            If Not fn_Ejecuta(String.Format("usp_ActualizarFirmasOP_stro {0}, {1}, {2} ", grdOrdenPago.DataKeys(gr.RowIndex)("nro_op"), "SD", IIf(sender.checked, -1, 0))) = 1 Then
                Mensaje.MuestraMensaje(Master.Titulo, "No se ha podido actualizar la firma del subdirector.", TipoMsg.Falla)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubdirector"), CheckBox).Checked = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaSubdirector"), Label).Visible = True
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, String.Format("chkFirmaSubdirector_CheckedChanged Error: {0}", ex.Message), TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, String.Format("chkFirmaSubdirector_CheckedChanged: {0}", ex.Message))
        End Try

    End Sub

    Protected Sub chkFirmaDirector_CheckedChanged(sender As Object, e As EventArgs)

        Try

            Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, CheckBox).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)

            If Not sender.checked Then

                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaDirector"), Label).Visible = True


                Select Case CInt(grdOrdenPago.DataKeys(gr.RowIndex)("NivelAutorizacion"))

                    Case 4

                        'Se retira firma de tesorería
                        TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox).Checked = False
                        chkFirmaTesoreria_CheckedChanged(TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox), Nothing)

                    Case 5

                        'Se retira firma de director
                        TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirectorGeneral"), CheckBox).Checked = False
                        chkFirmaDirectorGeneral_CheckedChanged(TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirectorGeneral"), CheckBox), Nothing)

                End Select
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = False
            Else

                'Si no existen las firmas necesarias que anteceden a esta no se puede firmar
                If Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox).Checked OrElse
                    Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubdirector"), CheckBox).Checked Then

                    Mensaje.MuestraMensaje(Master.Titulo, "Se debe de contar con la firma del solicitante y subdirección. ", TipoMsg.Falla)
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirector"), CheckBox).Checked = False

                Else
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaDirector"), Label).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = True
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chk_Rechazo"), CheckBox).Checked = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).SelectedIndex = 0
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lnk_SelMotivo"), Label).Visible = False
                End If

            End If

            If Not fn_Ejecuta(String.Format("usp_ActualizarFirmasOP_stro {0}, {1}, {2} ", grdOrdenPago.DataKeys(gr.RowIndex)("nro_op"), "D", IIf(sender.checked, -1, 0))) = 1 Then
                Mensaje.MuestraMensaje(Master.Titulo, "No se ha podido actualizar la firma del director.", TipoMsg.Falla)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirector"), CheckBox).Checked = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaDirector"), Label).Visible = True
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, String.Format("chkFirmaDirector_CheckedChanged Error: {0}", ex.Message), TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, String.Format("chkFirmaDirector_CheckedChanged: {0}", ex.Message))
        End Try

    End Sub

    Protected Sub chkFirmaDirectorGeneral_CheckedChanged(sender As Object, e As EventArgs)

        Try

            Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, CheckBox).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)

            If Not sender.checked Then

                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaDirectorGeneral"), Label).Visible = True

                'Se retira firma de tesorería
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox).Checked = False
                chkFirmaTesoreria_CheckedChanged(TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox), Nothing)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = False
            Else

                'Si no existen las firmas necesarias que anteceden a esta no se puede firmar
                If Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox).Checked OrElse
                    Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubdirector"), CheckBox).Checked OrElse
                    Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirector"), CheckBox).Checked Then

                    Mensaje.MuestraMensaje(Master.Titulo, "Se debe de contar con la firma del solicitante, subdirección y dirección. ", TipoMsg.Falla)
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirectorGeneral"), CheckBox).Checked = False

                Else
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaDirectorGeneral"), Label).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = True
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chk_Rechazo"), CheckBox).Checked = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).SelectedIndex = 0
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lnk_SelMotivo"), Label).Visible = False
                End If

            End If

            If Not fn_Ejecuta(String.Format("usp_ActualizarFirmasOP_stro {0}, {1}, {2} ", grdOrdenPago.DataKeys(gr.RowIndex)("nro_op"), "DG", IIf(sender.checked, -1, 0))) = 1 Then
                Mensaje.MuestraMensaje(Master.Titulo, "No se ha podido actualizar la firma del director general.", TipoMsg.Falla)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirectorGeneral"), CheckBox).Checked = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaDirectorGeneral"), Label).Visible = True
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, String.Format("chkFirmaDirectorGeneral_CheckedChanged Error: {0}", ex.Message), TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, String.Format("chkFirmaDirectorGeneral_CheckedChanged: {0}", ex.Message))
        End Try

    End Sub

    Protected Sub chkFirmaTesoreria_CheckedChanged(sender As Object, e As EventArgs)

        Dim bFirmasValidas As Boolean = True

        Try

            Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, CheckBox).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)

            If Not sender.checked Then
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaTesoreria"), Label).Visible = True
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = False
            Else

                'Se validaran si ya existen todas las firmas antecesoras para que tesorería pueda autorizar
                Select Case CInt(grdOrdenPago.DataKeys(gr.RowIndex)("NivelAutorizacion"))

                    Case 1

                        If Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox).Checked OrElse
                            Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaJefe"), CheckBox).Checked Then

                            bFirmasValidas = False

                        End If

                    Case 2

                        If Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox).Checked OrElse
                            Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubgerente"), CheckBox).Checked Then

                            bFirmasValidas = False

                        End If

                    Case 3

                        If Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox).Checked OrElse
                            Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubdirector"), CheckBox).Checked Then

                            bFirmasValidas = False

                        End If

                    Case 4

                        If Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox).Checked OrElse
                            Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubdirector"), CheckBox).Checked OrElse
                            Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirector"), CheckBox).Checked Then

                            bFirmasValidas = False

                        End If

                    Case 5

                        If Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox).Checked OrElse
                            Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubdirector"), CheckBox).Checked OrElse
                            Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirector"), CheckBox).Checked OrElse
                            Not TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirectorGeneral"), CheckBox).Checked Then

                            bFirmasValidas = False

                        End If

                End Select

                If Not bFirmasValidas Then
                    Mensaje.MuestraMensaje(Master.Titulo, "Se debe de contar con las firmas de todos los usuarios. ", TipoMsg.Falla)
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox).Checked = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaTesoreria"), Label).Visible = True
                Else
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaTesoreria"), Label).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox).Checked = True
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chk_Rechazo"), CheckBox).Checked = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).Visible = False
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).SelectedIndex = 0
                    TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lnk_SelMotivo"), Label).Visible = False
                End If

            End If

            If Not fn_Ejecuta(String.Format("usp_ActualizarFirmasOP_stro {0}, {1}, {2} ", grdOrdenPago.DataKeys(gr.RowIndex)("nro_op"), "T", IIf(sender.checked, -1, 0))) = 1 Then
                Mensaje.MuestraMensaje(Master.Titulo, "No se ha podido actualizar la firma.", TipoMsg.Falla)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox).Checked = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lblPendienteFirmaTesoreria"), Label).Visible = True
            Else
                Mensaje.MuestraMensaje(Master.Titulo, IIf(sender.checked, "Orden de pago habilitada para autorización.", "Se ha desautorizado la orden de pago."), TipoMsg.Advertencia)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkSeleccionOP"), CheckBox).Visible = IIf(sender.checked, True, False)
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkSeleccionOP"), CheckBox).Checked = False
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, String.Format("chkFirmaTesoreria_CheckedChanged Error: {0}", ex.Message), TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, String.Format("chkFirmaTesoreria_CheckedChanged: {0}", ex.Message))
        End Try

    End Sub

    Protected Sub chk_Rechazo_CheckedChanged(sender As Object, e As EventArgs)
        Try

            Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, CheckBox).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)
            Dim chkImp As CheckBox = TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkImpresion"), CheckBox)

            Dim chkSol = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSolicitante"), CheckBox)
            Dim chkJefe = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaJefe"), CheckBox)
            Dim chkSubDir = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubdirector"), CheckBox)
            Dim chkDir = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirector"), CheckBox)
            Dim chk_FirmaDirGral = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaDirectorGeneral"), CheckBox)
            Dim chk_Teso = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaTesoreria"), CheckBox)
            Dim chkSubGnt = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("chkFirmaSubgerente"), CheckBox)

            If sender.checked = True Then

                If chkImp.Enabled = True Then chkImp.Checked = True

                'O rechaza o firma, no puede hacer las 2 cosas.

                If chkSol.Checked = True And chkSol.Enabled = True Then chkSol.Checked = False
                If chkJefe.Checked = True And chkJefe.Enabled = True Then chkJefe.Checked = False
                If chkSubGnt.Checked = True And chkSubGnt.Enabled = True Then chkSubGnt.Checked = False
                If chkSubDir.Checked = True And chkSubDir.Enabled = True Then chkSubDir.Checked = False
                If chkDir.Checked = True And chkDir.Enabled = True Then chkDir.Checked = False
                If chk_FirmaDirGral.Checked = True And chk_FirmaDirGral.Enabled = True Then chk_FirmaDirGral.Checked = False
                If chk_Teso.Checked = True And chk_Teso.Enabled = True Then chk_Teso.Checked = False


                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lnk_SelMotivo"), Label).Visible = True
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).Visible = True
            Else
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("lnk_SelMotivo"), Label).Visible = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).Visible = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txtOtros"), TextBox).Visible = False
                TryCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList).SelectedIndex = 0
                chkImp.Checked = False
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "chk_Rechazo_CheckedChanged: " & ex.Message)
        End Try
    End Sub

    Private Sub CargaModalRoles(cod_rol As Integer)

        hid_Persona.Value = cod_rol

        Funciones.LlenaGrid(gvd_Destinatarios, Funciones.fn_ObtieneUsuarioRol(cod_rol, dtConsulta))

        If gvd_Destinatarios.Rows.Count > 0 Then
            Funciones.AbrirModal("#Destinatario")
        Else
            Mensaje.MuestraMensaje(Master.Titulo, "No existen usuarios asociados a este rol", TipoMsg.Advertencia)
        End If
    End Sub

    Protected Sub lnk_SelSolicita_Click(sender As Object, e As EventArgs)
        Try
            CargaModalRoles(Cons.TipoPersona.Solicitante)
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "lnk_SelSolicita_Click: " & ex.Message)
        End Try
    End Sub

    Protected Sub lnk_SelJefe_Click(sender As Object, e As EventArgs)
        Try
            CargaModalRoles(Cons.TipoPersona.JefeInmediato)
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "lnk_SelJefe_Click: " & ex.Message)
        End Try
    End Sub
    Protected Sub lnk_SelSubDir_Click(sender As Object, e As EventArgs)
        Try
            CargaModalRoles(Cons.TipoPersona.Subdirector)
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "lnk_SelSubDir_Click: " & ex.Message)
        End Try
    End Sub
    Protected Sub lnk_SelDir_Click(sender As Object, e As EventArgs)
        Try
            CargaModalRoles(Cons.TipoPersona.Director)
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "lnk_SelDir_Click: " & ex.Message)
        End Try
    End Sub
    Protected Sub lnk_SelTeso_Click(sender As Object, e As EventArgs)
        Try
            CargaModalRoles(Cons.TipoPersona.Tesoreria)
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "lnk_SelTeso_Click: " & ex.Message)
        End Try
    End Sub
    Protected Sub lnk_SelConta_Click(sender As Object, e As EventArgs)
        Try
            CargaModalRoles(Cons.TipoPersona.Contabilidad)
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "lnk_SelConta_Click: " & ex.Message)
        End Try
    End Sub

    'Protected Sub lnk_SelMotivo_Click(sender As Object, e As EventArgs)
    '    Try
    '        Dim Row As GridViewRow = DirectCast(DirectCast(DirectCast(sender, LinkButton).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)
    '        hid_IndexRechazo.Value = Row.RowIndex
    '        txt_MotivoRechazo.Text = TryCast(grdOrdenPago.Rows(Row.RowIndex).FindControl("txt_Motivo"), TextBox).Text

    '        If TryCast(grdOrdenPago.Rows(Row.RowIndex).FindControl("chk_Rechazo"), CheckBox).Enabled = False Then
    '            txt_MotivoRechazo.Enabled = False
    '            btn_GuardaMotivo.Visible = False
    '        Else
    '            txt_MotivoRechazo.Enabled = True
    '            btn_GuardaMotivo.Visible = True
    '        End If

    '        Funciones.AbrirModal("#Rechazo")
    '    Catch ex As Exception
    '        Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
    '        Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "lnk_SelMotivo_Click: " & ex.Message)
    '    End Try
    'End Sub

    'Private Function fn_ObtieneUsuarioRol(cod_rol As Integer, Optional ByVal intDefault As Integer = 0, Optional ByVal cod_usuario As String = vbNullString) As DataTable
    '    'Dim ws As New ws_FirmaDigital.FirmasDigitalClient

    '    'ListaResultado = ws.ObtieneUsuarioFirmaE(tPersona)
    '    'ddl_Destinatario.DataSource = ListaResultado

    '    fn_Consulta("spS_UsuarioXRol " & cod_rol & "," & intDefault & ",'" & cod_usuario & "'", dtConsulta)

    '    Return dtConsulta

    'End Function


    Private Sub DesHabilitaChecksFirma()
        Dim ws As New ws_Generales.GeneralesClient

        Dim AdminUsuStr As String

        For Each row In grdOrdenPago.Rows


            Dim chkSol = DirectCast(row.FindControl("chkFirmaSolicitante"), CheckBox)
            Dim chkJefe = DirectCast(row.FindControl("chkFirmaJefe"), CheckBox)
            Dim chkSubDir = DirectCast(row.FindControl("chkFirmaSubdirector"), CheckBox)
            Dim chkDir = DirectCast(row.FindControl("chkFirmaDirector"), CheckBox)
            Dim chk_FirmaDirGral = DirectCast(row.FindControl("chkFirmaDirectorGeneral"), CheckBox)
            Dim chk_Teso = DirectCast(row.FindControl("chkFirmaTesoreria"), CheckBox)
            Dim chkSubGnt = DirectCast(row.FindControl("chkFirmaSubgerente"), CheckBox)


            Dim lnk_SelSolicitante = DirectCast(row.FindControl("lnk_SelSolicitante"), LinkButton)
            Dim lnk_SelJefe = DirectCast(row.FindControl("lnk_SelJefe"), LinkButton)
            Dim lnk_SelSubDir = DirectCast(row.FindControl("lnk_SelSubDir"), LinkButton)
            Dim lnk_SelDir = DirectCast(row.FindControl("lnk_SelDir"), LinkButton)
            Dim lnk_SelDirGral = DirectCast(row.FindControl("lnk_SelDirGral"), LinkButton)
            Dim lnk_SelTeso = DirectCast(row.FindControl("lnk_SelTeso"), LinkButton)


            'No debe dejar hacer nada si no es usuario involucrado

            Dim chk_Impresion = DirectCast(row.FindControl("chkImpresion"), CheckBox)
            Dim chkRech = DirectCast(row.FindControl("chk_Rechazo"), CheckBox)
            chk_Impresion.Enabled = False
            chkRech.Enabled = False

            If grdOrdenPago.DataKeys(row.RowIndex)("Solicitante") = Master.cod_usuario And chkSol.Visible = True Then
                If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoSolicitante") = -1 Then
                    chk_Impresion.Enabled = False
                    chkRech.Enabled = False
                Else
                    chk_Impresion.Enabled = True
                    chkRech.Enabled = True
                End If
            End If

            If grdOrdenPago.DataKeys(row.RowIndex)("Jefe") = Master.cod_usuario And chkJefe.Visible = True Then
                If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoJefe") = -1 Then
                    chk_Impresion.Enabled = False
                    chkRech.Enabled = False
                    chkJefe.Enabled = False
                    lnk_SelSolicitante.Enabled = False
                    lnk_SelSolicitante.ForeColor = Drawing.Color.Gray
                Else
                    chk_Impresion.Enabled = True
                    chkRech.Enabled = True
                End If
            End If
            If grdOrdenPago.DataKeys(row.RowIndex)("Subgerente") = Master.cod_usuario And chkSubGnt.Visible = True Then
                If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoSubgerente") = -1 Then
                    chk_Impresion.Enabled = False
                    chkSubDir.Enabled = False
                    chkRech.Enabled = False
                    lnk_SelJefe.Enabled = False
                    lnk_SelJefe.ForeColor = Drawing.Color.Gray
                Else
                    chk_Impresion.Enabled = True
                    chkRech.Enabled = True
                End If
            End If

            If grdOrdenPago.DataKeys(row.RowIndex)("Subdirector") = Master.cod_usuario And chkSubDir.Visible = True Then
                If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoSubdirector") = -1 Then
                    chk_Impresion.Enabled = False
                    chkRech.Enabled = False
                    lnk_SelSubDir.Enabled = False
                    lnk_SelSubDir.ForeColor = Drawing.Color.Gray
                Else
                    chk_Impresion.Enabled = True
                    chkRech.Enabled = True
                End If
            End If

            If grdOrdenPago.DataKeys(row.RowIndex)("Director") = Master.cod_usuario And chkDir.Visible = True Then
                If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoDirector") = -1 Then
                    chk_Impresion.Enabled = False
                    chkRech.Enabled = False
                    chkDir.Enabled = False
                    lnk_SelDir.Enabled = False
                    lnk_SelDir.ForeColor = Drawing.Color.Gray
                Else
                    chk_Impresion.Enabled = True
                    chkRech.Enabled = True
                End If
            End If

            If grdOrdenPago.DataKeys(row.RowIndex)("DirectorGeneral") = Master.cod_usuario And chk_FirmaDirGral.Visible = True Then
                If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoDirectorGeneral") = -1 Then
                    chk_FirmaDirGral.Enabled = False
                    chk_Impresion.Enabled = False
                    chkRech.Enabled = False
                    lnk_SelDirGral.Enabled = False
                    lnk_SelDirGral.ForeColor = Drawing.Color.Gray
                Else
                    chk_Impresion.Enabled = True
                    chkRech.Enabled = True
                End If
            End If
            If grdOrdenPago.DataKeys(row.RowIndex)("Tesoreria") = Master.cod_usuario And chk_Teso.Visible = True Then
                If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoTesoreria") = -1 Then
                    chk_Impresion.Enabled = False
                    chkRech.Enabled = False
                    chk_Teso.Enabled = False
                    lnk_SelTeso.Enabled = False
                    lnk_SelTeso.ForeColor = Drawing.Color.Gray
                Else
                    chk_Impresion.Enabled = True
                    chkRech.Enabled = True
                End If
            End If


            AdminUsuStr = ws.ObtieneParametro(Cons.TargetFiltrosAdminStro)

            If InStr(AdminUsuStr, Master.cod_usuario) Then
                'If Master.cod_usuario = "CLOPEZ" Or Master.cod_usuario = "AMEZA" Or Master.cod_usuario = "CREYES" Then
                chk_Impresion.Enabled = True
                chkRech.Enabled = True
            End If



            'If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoSolicitante") = -1 Then
            '    chkSol.Enabled = False
            '    chkRech.Enabled = False
            'End If

            'If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoJefe") = -1 Then
            '    chkJefe.Enabled = False
            '    chkRech.Enabled = False
            '    lnk_SelSolicitante.Enabled = False
            '    lnk_SelSolicitante.ForeColor = Drawing.Color.Gray
            'End If

            'If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoSubgerente") = -1 Then
            '    chkSubDir.Enabled = False
            '    chkRech.Enabled = False
            '    lnk_SelJefe.Enabled = False
            '    lnk_SelJefe.ForeColor = Drawing.Color.Gray
            'End If

            'If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoSubdirector") = -1 Then
            '    chkSubDir.Enabled = False
            '    chkRech.Enabled = False
            '    lnk_SelSubDir.Enabled = False
            '    lnk_SelSubDir.ForeColor = Drawing.Color.Gray

            '    If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoDirector") = -1 Or grdOrdenPago.DataKeys(row.RowIndex)("FirmadoTesoreria") = -1 Then
            '        lnk_SelSubDir.Enabled = False
            '        lnk_SelSubDir.ForeColor = Drawing.Color.Gray
            '    End If
            'End If

            'If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoDirector") = -1 Then
            '    chkDir.Enabled = False
            '    chkRech.Enabled = False
            '    ' chk_AutorizaDireccion.Enabled = False
            'End If

            'If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoDirectorGeneral") = -1 Then
            '    chk_FirmaDirGral.Enabled = False
            '    chkRech.Enabled = False
            '    ' chk_AutorizaDirectorGral.Enabled = False
            'End If

            'If grdOrdenPago.DataKeys(row.RowIndex)("FirmadoTesoreria") = -1 Then
            '    chk_Teso.Enabled = False
            '    chkDir.Enabled = False
            '    chk_FirmaDirGral.Enabled = False
            '    'chk_AutorizaDireccion.Enabled = False
            '    'chk_AutorizaDirectorGral.Enabled = False

            '    lnk_SelTeso.Enabled = False
            '    lnk_SelTeso.ForeColor = Drawing.Color.Gray
            '    lnk_SelDir.Enabled = False
            '    lnk_SelDir.ForeColor = Drawing.Color.Gray
            '    lnk_SelDirGral.Enabled = False
            '    lnk_SelDirGral.ForeColor = Drawing.Color.Gray
            'End If

            If grdOrdenPago.DataKeys(row.RowIndex)("Rechazada") = 1 Then
                chkRech.Enabled = False
                chkJefe.Enabled = False
                chkSubDir.Enabled = False
                chkDir.Enabled = False
                chk_FirmaDirGral.Enabled = False
                chk_Teso.Enabled = False
                chk_Impresion.Enabled = False

                '        chk_Manual.Enabled = False
                '        chk_AutorizaDireccion.Enabled = False
                '        chk_AutorizaDirectorGral.Enabled = False

                lnk_SelSolicitante.Enabled = False
                lnk_SelSolicitante.ForeColor = Drawing.Color.Gray
                lnk_SelJefe.Enabled = False
                lnk_SelJefe.ForeColor = Drawing.Color.Gray
                lnk_SelSubDir.Enabled = False
                lnk_SelSubDir.ForeColor = Drawing.Color.Gray
                lnk_SelDir.Enabled = False
                lnk_SelDir.ForeColor = Drawing.Color.Gray
                lnk_SelDirGral.Enabled = False
                lnk_SelDirGral.ForeColor = Drawing.Color.Gray
                'lnk_SelSubGnt.Enabled = False
                'lnk_SelSubGnt.ForeColor = Drawing.Color.Gray

                'Else
                'lnk_SelMotivo.Visible = False
            End If

        Next
    End Sub

    Private Sub grdOrdenPago_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grdOrdenPago.PageIndexChanging
        Try
            grdOrdenPago.PageIndex = e.NewPageIndex
            Funciones.LlenaGrid(grdOrdenPago, ConsultaOrdenesPagoSiniestros(Cons.StrosTradicional))
            ' Funciones.LlenaGrid(grdOrdenPago, ActualizaDataOP)
            'ListaRamosContables()
            'DesHabilitaChecksFirma()
            'Funciones.EjecutaFuncion("fn_EstadoFilas('grdOrdenPago',true);")

            'ListaRamosContables()
            'DesHabilitaChecksFirma()

            EstadoDetalleOrden()
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "grdOrdenPago: " & ex.Message)
        End Try
    End Sub

    Private Function fn_EjecutaPolitica(ByVal cod_moneda_pol As Integer, ByVal monto_maximo As String, ByVal cod_moneda_op As Integer, ByVal monto_op As Double) As Boolean
        Dim monto_consolidado As Double

        fn_EjecutaPolitica = False

        If cod_moneda_pol = 0 And cod_moneda_op = 1 Then 'Si la validacion es en pesos con documento en Dolares
            monto_consolidado = monto_op * fn_TipoCambio(Now.ToString("yyyyMMdd"), 1)
        ElseIf cod_moneda_pol = 1 And cod_moneda_op = 0 Then 'Si la validacion es en dolares con documento en Pesos
            monto_consolidado = monto_op / fn_TipoCambio(Now.ToString("yyyyMMdd"), 1)
        Else 'Si se trata de las mismas monedas
            monto_consolidado = monto_op
        End If

        If monto_maximo <> "ILIMITADO" And monto_consolidado > CDbl(Replace(monto_maximo, ",", "")) Then
            fn_EjecutaPolitica = True
        End If

    End Function

    Private Sub btnCambia_Click(sender As Object, e As EventArgs) Handles btnCambia.Click
        'Dim ws As New ws_FirmaDigital.FirmasDigitalClient
        'Dim Resultado As IList = Nothing

        Dim CodRol As Integer
        Dim sn_seleccion = vbNullString
        Dim cod_usuario As String = vbNullString
        Dim usuario As String = vbNullString
        Dim dato_firma() As Byte = Nothing
        Dim usuario_id As String = vbNullString
        Dim mail As String = vbNullString
        Dim chk_control As String = vbNullString
        Dim lnk_control As String = vbNullString
        Dim img_control As String = vbNullString
        Dim campo As String = vbNullString
        Dim clave As String = vbNullString
        Dim nombre As String = vbNullString
        Dim firma As String = vbNullString
        Dim id_net As String = vbNullString
        Dim email As String = vbNullString


        Try

            CodRol = hid_Persona.Value

            Select Case CodRol
                Case 5800
                    chk_control = "chk_FirmaSol"
                    lnk_control = "lnk_SelSolicitante"
                    img_control = "img_FirmaSolicita"
                    sn_seleccion = "sn_solicita"
                    campo = "sn_firma_solicita"
                    clave = "cod_usuario_solicita"
                    nombre = "usuario_solicita"
                    firma = "firma_solicita"
                    id_net = "user_id_solicita"
                    email = "mail_solicita"
                Case 5801
                    chk_control = "chk_FirmaJefe"
                    lnk_control = "lnk_SelJefe"
                    img_control = "img_FirmaJefe"
                    sn_seleccion = "sn_jefe_inmediato"
                    campo = "sn_firma_jefe"
                    clave = "cod_usuario_jefe"
                    nombre = "usuario_jefe"
                    firma = "firma_jefe"
                    id_net = "user_id_jefe"
                    email = "mail_jefe"
                Case 5802
                    chk_control = "chk_SubDir"
                    lnk_control = "lnk_SelSubDir"
                    img_control = "img_FirmaSubditector"
                    sn_seleccion = "sn_subdirector"
                    campo = "sn_firma_subdirector"
                    clave = "cod_usuario_subdirector"
                    nombre = "usuario_subdirector"
                    firma = "firma_subdirector"
                    id_net = "user_id_subdirector"
                    email = "mail_subdirector"
                Case 5803
                    chk_control = "chk_FirmaDir"
                    lnk_control = "lnk_SelDir"
                    img_control = "img_FirmaDirector"
                    sn_seleccion = "sn_director"
                    campo = "sn_firma_director"
                    clave = "cod_usuario_director"
                    nombre = "usuario_director"
                    firma = "firma_director"
                    id_net = "user_id_director"
                    email = "mail_director"
                Case 5804
                    chk_control = "chk_FirmaTeso"
                    lnk_control = "lnk_SelTeso"
                    img_control = "img_FirmaTesoreria"
                    sn_seleccion = "sn_tesoreria"
                    campo = "sn_firma_tesoreria"
                    clave = "cod_usuario_tesoreria"
                    nombre = "usuario_tesoreria"
                    firma = "firma_tesoreria"
                    id_net = "user_id_tesoreria"
                    email = "mail_tesoreria"
                Case 5805
                    chk_control = "chk_FirmaCon"
                    lnk_control = "lnk_SelConta"
                    img_control = "img_FirmaContabilidad"
                    sn_seleccion = "sn_contabilidad"
                    campo = "sn_firma_contabilidad"
                    clave = "cod_usuario_contabilidad"
                    nombre = "usuario_contabilidad"
                    firma = "firma_contabilidad"
                    id_net = "user_id_contabilidad"
                    email = "mail_contabilidad"
                Case 5807
                    chk_control = "chk_FirmaDirGral"
                    lnk_control = "lnk_SelDirGral"
                    img_control = "img_FirmaDirectorGral"
                    sn_seleccion = "sn_director_gral"
                    campo = "sn_firma_director_gral"
                    clave = "cod_usuario_director_gral"
                    nombre = "usuario_director_gral"
                    firma = "firma_director_gral"
                    id_net = "user_id_director_gral"
                    email = "mail_director_gral"
            End Select

            'Resultado = ws.ActualizaDestinatarioFirma(ddl_Destinatario.SelectedValue.ToString, CodRol)
            'If Resultado(0) = 0 Then

            For Each row In gvd_Destinatarios.Rows
                If TryCast(row.FindControl("chk_Predeterminado"), CheckBox).Checked = True Then
                    cod_usuario = TryCast(row.FindControl("txt_Clave"), TextBox).Text
                    usuario = TryCast(row.FindControl("txt_Nombre"), TextBox).Text
                    dato_firma = gvd_Destinatarios.DataKeys(row.rowIndex)("firma")
                    usuario_id = gvd_Destinatarios.DataKeys(row.rowIndex)("usuario_id")
                    mail = gvd_Destinatarios.DataKeys(row.rowIndex)("mail")
                    Exit For
                End If
            Next


            If fn_Ejecuta("spU_RolPredeterminado '" & cod_usuario & "'," & CodRol) = 1 Then
                For Each row In grdOrdenPago.Rows

                    If grdOrdenPago.DataKeys(row.RowIndex)(campo) = 0 Then
                        TryCast(row.FindControl(Replace(lnk_control, "lnk_Sel", "hid_Cod")), HiddenField).Value = cod_usuario
                        TryCast(row.FindControl(lnk_control), LinkButton).Text = usuario
                        TryCast(row.FindControl(img_control), Image).ImageUrl = "data:image/png;base64," + Convert.ToBase64String(dato_firma)
                        TryCast(row.FindControl(Replace(lnk_control, "lnk", "hid")), HiddenField).Value = usuario_id
                        TryCast(row.FindControl(Replace(lnk_control, "lnk_Sel", "hid_Mail")), HiddenField).Value = mail

                        'Politicas de autorización si se ha seleccionado la OP para firma
                        If TryCast(row.FindControl(chk_control), CheckBox).Checked = True Then

                            If CodRol = 5802 Then

                                If Funciones.fn_ObtieneUsuarioRol(Cons.TipoPersona.Subdirector, dtConsulta, -1, TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("hid_CodSubDir"), HiddenField).Value).Rows.Count > 0 Then

                                    If fn_EjecutaPolitica(dtConsulta.Rows(0)("cod_moneda"),
                                          dtConsulta.Rows(0)("monto_maximo"),
                                          grdOrdenPago.DataKeys(row.RowIndex)("cod_moneda"),
                                          grdOrdenPago.DataKeys(row.RowIndex)("Monto")) = True Or
                                          TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_Financiado"), CheckBox).Checked = True Or
                                          TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_Urgente"), CheckBox).Checked = True Then

                                        TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_AutorizaDireccion"), CheckBox).Checked = True
                                        TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_AutorizaDireccion"), CheckBox).Enabled = False
                                    Else
                                        TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_AutorizaDireccion"), CheckBox).Checked = False
                                        TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_AutorizaDireccion"), CheckBox).Enabled = True
                                    End If

                                End If

                                fn_Ejecuta("spU_AutorizaDirector " & grdOrdenPago.DataKeys(row.RowIndex)("nro_op") & "," &
                                                                        IIf(TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_AutorizaDireccion"), CheckBox).Checked = True, -1, 0))

                            ElseIf CodRol = 5803 Then

                                If Funciones.fn_ObtieneUsuarioRol(Cons.TipoPersona.Director, dtConsulta, -1, TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("hid_CodDir"), HiddenField).Value).Rows.Count > 0 Then

                                    If fn_EjecutaPolitica(dtConsulta.Rows(0)("cod_moneda"),
                                          dtConsulta.Rows(0)("monto_maximo"),
                                          grdOrdenPago.DataKeys(row.RowIndex)("cod_moneda"),
                                          grdOrdenPago.DataKeys(row.RowIndex)("Monto")) = True Then

                                        TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_AutorizaDirectorGral"), CheckBox).Checked = True
                                        TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_AutorizaDirectorGral"), CheckBox).Enabled = False
                                    Else
                                        TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_AutorizaDirectorGral"), CheckBox).Checked = False
                                        TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_AutorizaDirectorGral"), CheckBox).Enabled = True
                                    End If

                                End If

                                fn_Ejecuta("spU_AutorizaDirectorGral " & grdOrdenPago.DataKeys(row.RowIndex)("nro_op") & "," &
                                                                         IIf(TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_AutorizaDirectorGral"), CheckBox).Checked = True, -1, 0))

                            ElseIf CodRol = 5805 Then

                                If Funciones.fn_ObtieneUsuarioRol(Cons.TipoPersona.Contabilidad, dtConsulta, -1, TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("hid_CodConta"), HiddenField).Value).Rows.Count > 0 Then

                                    'Verifica si se activa la Politica de Montos
                                    If fn_EjecutaPolitica(dtConsulta.Rows(0)("cod_moneda"),
                                                          dtConsulta.Rows(0)("monto_maximo"),
                                                          grdOrdenPago.DataKeys(row.RowIndex)("cod_moneda"),
                                                          grdOrdenPago.DataKeys(row.RowIndex)("Monto")) = True Then

                                        TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("chk_FirmaCon"), CheckBox).Checked = False
                                        TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("img_BlankContabilidad"), Image).Visible = True
                                        TryCast(grdOrdenPago.Rows(row.RowIndex).FindControl("img_FirmaContabilidad"), Image).Visible = False
                                    End If

                                End If

                            End If
                        End If
                    End If
                Next

                Dim RowOP() As Data.DataRow
                RowOP = dtOrdenPago.Select(campo & " = 0 ")

                For Each fila In RowOP
                    fila(clave) = cod_usuario
                    fila(nombre) = usuario
                    fila(firma) = dato_firma
                    fila(id_net) = usuario_id
                    fila(email) = mail
                    fila(sn_seleccion) = 0
                Next

                Funciones.CerrarModal("#Destinatario")
                Mensaje.MuestraMensaje(Master.Titulo, "Se actualizó correctamente el rol", TipoMsg.Confirma)

                Funciones.fn_InsertaBitacora(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "Actualización de Rol: " & CodRol)
            End If


        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btnCambia_Click: " & ex.Message)
        End Try
    End Sub

    'Private Sub btn_AddPol_Click(sender As Object, e As EventArgs) Handles btn_AddPol.Click
    '    Try
    '        Master.MuestraPolizario("gvd_Poliza", False, False, False, False, False)
    '    Catch ex As Exception
    '        Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
    '        Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_AddPol_Click: " & ex.Message)
    '    End Try
    'End Sub

    Private Sub btn_Imprimir_Click(sender As Object, e As EventArgs) Handles btn_Imprimir.Click
        Dim OPS() As String = Nothing
        Dim Index As Integer = 0
        Dim pdf = New reportePDF
        Dim RutaArchivo As String()
        Dim seleccionados As Integer = 0
        Dim server As String = String.Empty
        Try
            'ActualizaDataOP()
            Dim ws As New ws_Generales.GeneralesClient

            server = ws.ObtieneParametro(Cons.TargetReport)
            server = Replace(Replace(server, "@Reporte", "OrdenPago"), "@Formato", "PDF") & "&nro_op=@nro_op"
            server = Replace(server, Cons.ReposSource, Cons.ReposReport)
            server = Replace(server, "OrdenPago", "OrdenPago_stro")

            For Each row In grdOrdenPago.Rows
                If TryCast(row.FindControl("chk_Print"), CheckBox).Checked = True Then
                    seleccionados = seleccionados + 1
                End If
            Next

            ReDim OPS(seleccionados - 1)

            For Each row In grdOrdenPago.Rows
                If TryCast(row.FindControl("chk_Print"), CheckBox).Checked = True Then
                    OPS(Index) = DirectCast(row.FindControl("lblOrdenPago"), Label).Text.Trim
                    Index = Index + 1
                End If
            Next

            If OPS.Length > 0 AndAlso Index <> 0 Then
                pdf.Cod_usuario = Master.cod_usuario.ToString()
                pdf.Nro_ops = OPS
                pdf.GenerarRenombrar()
                '>VZAVALETA_10290_CC7_PDF 
                Funciones.EjecutaFuncion("fn_abrir_documento('" + pdf.RutaArchivo_correo + "')")
            Else
                Index = 0
                MuestraMensaje("Validación", "Error al imprimir el PDF", TipoMsg.Falla)
            End If


        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Imprimir_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub grdOrdenPago_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grdOrdenPago.RowCommand
        Try
            If e.CommandName = "OrdenPago" Then
                Dim Index As Integer = e.CommandSource.NamingContainer.RowIndex
                Master.ConsultaOrdenPago(grdOrdenPago.DataKeys(Index)("nro_op"))
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "grdOrdenPago_RowCommand: " & ex.Message)
        End Try
    End Sub

    Protected Sub lnk_SelDirGral_Click(sender As Object, e As EventArgs)
        Try
            CargaModalRoles(Cons.TipoPersona.DirectorGeneral)
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "lnk_SelDirGral_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub grdOrdenPago_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grdOrdenPago.RowDataBound
        Try
            Dim Tabla_Asegurado As String
            Dim Asegurados() As String


            If e.Row.RowType = DataControlRowType.DataRow Then

                'Tabla_Asegurado = "<table style='width:100%;'>"
                'Asegurados = Split(sender.DataKeys(e.Row.RowIndex)("Asegurados"), "|")
                'For Each item In Asegurados
                '    Tabla_Asegurado = Tabla_Asegurado & "<tr>"
                '    Tabla_Asegurado = Tabla_Asegurado & "<td style='width:100%;'>-" & Mid(Trim(item), 1, 35) & IIf(Len(item) > 35, "...", "") & "</td>"
                '    Tabla_Asegurado = Tabla_Asegurado & "</tr>"
                'Next
                'Tabla_Asegurado = Tabla_Asegurado & "</table>"
                'CType(e.Row.FindControl("div_Asegurado"), HtmlGenericControl).InnerHtml = Tabla_Asegurado


                'If sender.DataKeys(e.Row.RowIndex)("sn_subdirector") = 1 And sender.DataKeys(e.Row.RowIndex)("sn_firma_subdirector") = 0 Then
                '    If Funciones.fn_ObtieneUsuarioRol(Cons.TipoPersona.Subdirector, dtConsulta, 0, sender.DataKeys(e.Row.RowIndex)("cod_usuario_subdirector")).Rows.Count > 0 Then

                '        If fn_EjecutaPolitica(dtConsulta.Rows(0)("cod_moneda"),
                '                              dtConsulta.Rows(0)("monto_maximo"),
                '                              grdOrdenPago.DataKeys(e.Row.RowIndex)("cod_moneda"),
                '                              grdOrdenPago.DataKeys(e.Row.RowIndex)("Monto")) = True Or
                '            TryCast(e.Row.FindControl("chk_Financiado"), CheckBox).Checked = True Or
                '            TryCast(e.Row.FindControl("chk_Urgente"), CheckBox).Checked = True Then

                '            TryCast(e.Row.FindControl("chk_AutorizaDireccion"), CheckBox).Checked = True
                '            TryCast(e.Row.FindControl("chk_AutorizaDireccion"), CheckBox).Enabled = False

                '            fn_Ejecuta("spU_AutorizaDirector " & sender.DataKeys(e.Row.RowIndex)("nro_op") & ",-1")
                '        End If
                '    End If
                'End If


                'If sender.DataKeys(e.Row.RowIndex)("sn_director") = 1 And sender.DataKeys(e.Row.RowIndex)("sn_firma_director") = 0 Then
                '    If Funciones.fn_ObtieneUsuarioRol(Cons.TipoPersona.Director, dtConsulta, 0, sender.DataKeys(e.Row.RowIndex)("cod_usuario_director")).Rows.Count > 0 Then

                '        If fn_EjecutaPolitica(dtConsulta.Rows(0)("cod_moneda"),
                '                              dtConsulta.Rows(0)("monto_maximo"),
                '                              grdOrdenPago.DataKeys(e.Row.RowIndex)("cod_moneda"),
                '                              grdOrdenPago.DataKeys(e.Row.RowIndex)("Monto")) = True Then

                '            TryCast(e.Row.FindControl("chk_AutorizaDirectorGral"), CheckBox).Checked = True
                '            TryCast(e.Row.FindControl("chk_AutorizaDirectorGral"), CheckBox).Enabled = False

                '            fn_Ejecuta("spU_AutorizaDirectorGral " & sender.DataKeys(e.Row.RowIndex)("nro_op") & ",-1")
                '        End If
                '    End If
                'End If

                'If sender.DataKeys(e.Row.RowIndex)("sn_contabilidad") = 1 And sender.DataKeys(e.Row.RowIndex)("sn_firma_contabilidad") = 0 Then
                '    If Funciones.fn_ObtieneUsuarioRol(Cons.TipoPersona.Contabilidad, dtConsulta, 0, sender.DataKeys(e.Row.RowIndex)("cod_usuario_contabilidad")).Rows.Count > 0 Then

                '        If fn_EjecutaPolitica(dtConsulta.Rows(0)("cod_moneda"),
                '                              dtConsulta.Rows(0)("monto_maximo"),
                '                              grdOrdenPago.DataKeys(e.Row.RowIndex)("cod_moneda"),
                '                              grdOrdenPago.DataKeys(e.Row.RowIndex)("Monto")) = True Then

                '            TryCast(e.Row.FindControl("chk_FirmaCon"), CheckBox).Checked = False
                '            TryCast(e.Row.FindControl("img_BlankContabilidad"), Image).Visible = True
                '            TryCast(e.Row.FindControl("img_FirmaContabilidad"), Image).Visible = False
                '        End If
                '    End If
                'End If

                ''Si fue eleminada
                'If sender.DataKeys(e.Row.RowIndex)("cod_estatus_op") = 6 Then
                '    TryCast(e.Row.FindControl("lbl_OrdenPago"), Label).ForeColor = Drawing.Color.OrangeRed
                '    CType(e.Row.FindControl("div_Asegurado"), HtmlGenericControl).Attributes.Add("style", "color: OrangeRed;")
                '    TryCast(e.Row.FindControl("lbl_BroCia"), Label).ForeColor = Drawing.Color.OrangeRed
                '    TryCast(e.Row.FindControl("lbl_Moneda"), Label).ForeColor = Drawing.Color.OrangeRed
                '    TryCast(e.Row.FindControl("lbl_Monto"), Label).ForeColor = Drawing.Color.OrangeRed
                '    TryCast(e.Row.FindControl("lbl_OrdenPago"), Label).ToolTip = "Orden de Pago Anulada"

                '    TryCast(e.Row.FindControl("lbl_Anulada"), Label).Visible = True
                '    TryCast(e.Row.FindControl("lbl_Financiado"), Label).Visible = False

                '    TryCast(e.Row.FindControl("chk_Financiado"), CheckBox).Enabled = False
                '    TryCast(e.Row.FindControl("chk_Urgente"), CheckBox).Enabled = False
                '    TryCast(e.Row.FindControl("chk_Manual"), CheckBox).Enabled = False
                '    TryCast(e.Row.FindControl("chk_AutorizaDireccion"), CheckBox).Enabled = False

                '    TryCast(e.Row.FindControl("lnk_SelJefe"), LinkButton).Enabled = False
                '    TryCast(e.Row.FindControl("lnk_SelTeso"), LinkButton).Enabled = False
                '    TryCast(e.Row.FindControl("lnk_SelConta"), LinkButton).Enabled = False
                '    TryCast(e.Row.FindControl("lnk_SelSolicitante"), LinkButton).Enabled = False
                '    TryCast(e.Row.FindControl("lnk_SelDir"), LinkButton).Enabled = False
                '    TryCast(e.Row.FindControl("lnk_SelSubDir"), LinkButton).Enabled = False
                '    TryCast(e.Row.FindControl("lnk_SelDirGral"), LinkButton).Enabled = False
                'End If

                'If sender.DataKeys(e.Row.RowIndex)("sn_financiado") = 1 Then
                '    TryCast(e.Row.FindControl("lbl_Financiado"), Label).Visible = True
                '    TryCast(e.Row.FindControl("chk_AutorizaDireccion"), CheckBox).Enabled = False
                'End If

                'If sender.DataKeys(e.Row.RowIndex)("sn_firma_subdirector") = 1 Then
                '    TryCast(e.Row.FindControl("chk_AutorizaDireccion"), CheckBox).Enabled = False
                'End If

                'If sender.DataKeys(e.Row.RowIndex)("sn_firma_tesoreria") = 1 Then
                '    TryCast(e.Row.FindControl("chk_AutorizaDireccion"), CheckBox).Enabled = False
                '    TryCast(e.Row.FindControl("chk_FirmaDir"), CheckBox).Enabled = False
                '    TryCast(e.Row.FindControl("chk_AutorizaDirectorGral"), CheckBox).Enabled = False
                '    TryCast(e.Row.FindControl("chk_FirmaDirGral"), CheckBox).Enabled = False
                'End If

                'If sender.DataKeys(e.Row.RowIndex)("sn_firma_contabilidad") = 1 Then
                '    TryCast(e.Row.FindControl("chk_Manual"), CheckBox).Enabled = False
                '    TryCast(e.Row.FindControl("chk_Rechazo"), CheckBox).Enabled = False
                'End If

                'Llenado del grid de detalles de la factura de la orden de pago
                Funciones.fn_Consulta(String.Format("usp_DetalleOrdenPagoFactura_stro {0}", sender.DataKeys(e.Row.RowIndex)("nro_op")), dtConsulta)

                If Not dtConsulta Is Nothing AndAlso dtConsulta.Rows.Count > 0 Then
                    Funciones.LlenaGrid(TryCast(e.Row.FindControl("grdDetalleFactura"), GridView), dtConsulta)
                End If

                dtConsulta = Nothing

                'Llenado del grid de la contabilidad en transito
                Funciones.fn_Consulta(String.Format("usp_DetalleOrdenPago_stro {0}", sender.DataKeys(e.Row.RowIndex)("nro_op")), dtConsulta)

                If Not dtConsulta Is Nothing AndAlso dtConsulta.Rows.Count > 0 Then
                    Funciones.LlenaGrid(TryCast(e.Row.FindControl("grdContabilidadTransito"), GridView), dtConsulta)
                End If

                dtConsulta = Nothing

                ''Llenado del grid de las firmas digitales
                'Funciones.fn_Consulta(String.Format("usp_ObtenerFirmasDigitalesOP_stro {0}", sender.DataKeys(e.Row.RowIndex)("nro_op")), dtConsulta)

                'If Not dtConsulta Is Nothing AndAlso dtConsulta.Rows.Count > 0 Then
                '    Funciones.LlenaGrid(TryCast(e.Row.FindControl("grdFirmasOP"), GridView), dtConsulta)
                'End If

                'dtConsulta = Nothing

                'If dtConsulta.Rows.Count > 0 Then
                '    'Dim query = From row In dtConsulta.AsEnumerable()
                '    '            Group row By nro_op = row("nro_op")
                '    '            Into Total = Group
                '    '            Select New With {
                '    '                                Key .MontoToal = Total.Sum(Function(r) r.Field(Of Decimal)("imp_mo"))
                '    '                            }

                '    'TryCast(e.Row.FindControl("txt_MontoTotal"), TextBox).Text = String.Format("{0:#,#0.00}", query(0).MontoToal)
                'End If

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "grdOrdenPago_RowDataBound: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Todas_Click(sender As Object, e As EventArgs) Handles btn_Todas.Click

        Try

            For Each row In grdOrdenPago.Rows
                Dim chkImp As CheckBox = TryCast(row.FindControl("chkImpresion"), CheckBox)
                If chkImp.Enabled = True Then
                    chkImp.Checked = True
                Else
                    chkImp.Checked = False
                End If
            Next

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Todas_Click: " & ex.Message)
        End Try

    End Sub

    Private Sub btn_Ninguna_Click(sender As Object, e As EventArgs) Handles btn_Ninguna.Click

        Try

            For Each row In grdOrdenPago.Rows
                Dim chkImp As CheckBox = TryCast(row.FindControl("chkImpresion"), CheckBox)
                If chkImp.Enabled = True Then
                    chkImp.Checked = False
                Else
                    chkImp.Checked = False
                End If
            Next

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Ninguna_Click: " & ex.Message)
        End Try

    End Sub

    'Private Sub tim_Actualizacion_Tick(sender As Object, e As EventArgs) Handles tim_Actualizacion.Tick
    '    Try
    '        Funciones.LlenaGrid(grdOrdenPago, ConsultaOrdenesPagoSiniestros(cmbModuloOP.SelectedValue))

    '        If grdOrdenPago.Rows.Count > 0 Then
    '            'ListaRamosContables()
    '            DesHabilitaChecksFirma()
    '            Funciones.EjecutaFuncion("fn_EstadoFilas('grdOrdenPago',true);")
    '        End If

    '    Catch ex As Exception
    '        Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
    '        Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "tim_Actualizacion_Tick: " & ex.Message)
    '    End Try
    'End Sub


    Protected Sub chk_Pendiente_CheckedChanged(sender As Object, e As EventArgs)
        VerificaRadios(Cons.TipoFiltro.Pendientes)
    End Sub
    Protected Sub chk_Autorizada_CheckedChanged(sender As Object, e As EventArgs)
        VerificaRadios(Cons.TipoFiltro.Firmadas)
    End Sub

    Private Sub VerificaRadios(tipo As Integer)

        Select Case tipo
            Case 0
                If chk_Todas.Checked Then
                    chk_Rechazadas.Checked = False
                    chk_PorRevisar.Checked = False
                    chk_Pendiente.Checked = False
                    chk_Autorizada.Checked = False
                    chk_FinalAut.Checked = False
                    chk_Revisadas.Checked = False
                    ddlRolFilter.Visible = False
                    div_Fechas.Visible = False
                End If
            Case 1
                If chk_PorRevisar.Checked Then
                    chk_Rechazadas.Checked = False
                    chk_Pendiente.Checked = False
                    chk_Todas.Checked = False
                    chk_Autorizada.Checked = False
                    chk_FinalAut.Checked = False
                    chk_Revisadas.Checked = False
                    ddlRolFilter.Visible = False
                    div_Fechas.Visible = False
                End If
            Case 2
                If chk_Pendiente.Checked Then
                    chk_Todas.Checked = False
                    chk_PorRevisar.Checked = False
                    chk_Autorizada.Checked = False
                    chk_Rechazadas.Checked = False
                    chk_FinalAut.Checked = False
                    chk_Revisadas.Checked = False
                    ddlRolFilter.Visible = True
                    div_Fechas.Visible = False
                End If
            Case 3
                If chk_Autorizada.Checked Then
                    chk_Todas.Checked = False
                    chk_PorRevisar.Checked = False
                    chk_Pendiente.Checked = False
                    chk_Rechazadas.Checked = False
                    chk_FinalAut.Checked = False
                    chk_Revisadas.Checked = False
                    ddlRolFilter.Visible = True
                    div_Fechas.Visible = False
                End If
            Case 4
                If chk_Rechazadas.Checked Then
                    chk_Todas.Checked = False
                    chk_PorRevisar.Checked = False
                    chk_Pendiente.Checked = False
                    chk_Autorizada.Checked = False
                    chk_FinalAut.Checked = False
                    chk_Revisadas.Checked = False
                    ddlRolFilter.Visible = False
                    div_Fechas.Visible = True
                End If
            Case 5
                If chk_FinalAut.Checked Then
                    chk_Rechazadas.Checked = False
                    chk_Todas.Checked = False
                    chk_PorRevisar.Checked = False
                    chk_Pendiente.Checked = False
                    chk_Autorizada.Checked = False
                    chk_Revisadas.Checked = False
                    ddlRolFilter.Visible = False
                    div_Fechas.Visible = True
                End If
            Case 6
                If chk_Revisadas.Checked Then
                    chk_Rechazadas.Checked = False
                    chk_Todas.Checked = False
                    chk_PorRevisar.Checked = False
                    chk_Pendiente.Checked = False
                    chk_Autorizada.Checked = False
                    chk_FinalAut.Checked = False
                    ddlRolFilter.Visible = False
                    div_Fechas.Visible = True
                End If
        End Select

    End Sub

    Private Sub lnkAceptarProc_Click(sender As Object, e As EventArgs) Handles lnkAceptarProc.Click
        Try
            ' ActualizaDataOP()
            If txtToken.Text = "" Then
                Mensaje.MuestraMensaje("Token", "Debe capturar número de token que llego a su correo para autorizar", TipoMsg.Advertencia)
            Else
                If hid_Token.Value = txtToken.Text Then
                    If fn_Autorizaciones(True) = False Then
                        Exit Sub
                    End If
                Else
                    Mensaje.MuestraMensaje("Valida Token", "El código proporcionado no es válido", TipoMsg.Falla)

                End If

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Firmar_Click: " & ex.Message)
        End Try
    End Sub

    Protected Sub chk_Todas_CheckedChanged(sender As Object, e As EventArgs)
        VerificaRadios(Cons.TipoFiltro.Todas)
    End Sub
    Protected Sub chk_PorRevisar_CheckedChanged(sender As Object, e As EventArgs)
        VerificaRadios(Cons.TipoFiltro.PorRevisar)
    End Sub
    Protected Sub chk_Rechazadas_CheckedChanged(sender As Object, e As EventArgs)
        VerificaRadios(Cons.TipoFiltro.Rechazadas)
    End Sub

    Protected Sub chk_FinalAut_CheckedChanged(sender As Object, e As EventArgs)
        VerificaRadios(Cons.TipoFiltro.Autorizadas)

    End Sub

    Protected Sub chk_Revisadas_CheckedChanged(sender As Object, e As EventArgs)
        VerificaRadios(Cons.TipoFiltro.Revisadas)
    End Sub

    Private Sub btn_SelTodos_Click(sender As Object, e As EventArgs) Handles btn_SelTodos.Click
        Try

            For Each row In grdOrdenPago.Rows
                Dim chkImp As CheckBox = TryCast(row.FindControl("chk_Print"), CheckBox)
                If chkImp.Enabled = True Then
                    chkImp.Checked = True
                Else
                    chkImp.Checked = False
                End If
            Next

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Todas_Click: " & ex.Message)
        End Try

    End Sub

    Protected Sub txt_Motivo_SelectedIndexChanged(sender As Object, e As EventArgs)


        Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, DropDownList).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)

        Dim ddlRechazo = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList)
        Dim txtOtros = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txtOtros"), TextBox)

        If ddlRechazo.SelectedValue = 11 Then
            'Mensaje.MuestraMensaje(Master.Titulo, "hola mundo", TipoMsg.Falla)
            txtOtros.Visible = True
        Else
            txtOtros.Visible = False
            txtOtros.Text = ""
        End If

    End Sub


End Class

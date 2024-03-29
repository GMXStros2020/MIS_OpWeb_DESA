Imports System.Data
Imports System.Data.SqlClient
Imports Mensaje
Imports Funciones
Imports System.IO

Partial Class Siniestros_AutElectTrad
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

    Public Property dtSegRev() As DataTable
        Get
            Return Session("dtSegRev")
        End Get
        Set(ByVal value As DataTable)
            Session("dtSegRev") = value
        End Set
    End Property
    Public Property dtNoProc() As DataTable
        Get
            Return Session("dtNoProc")
        End Get
        Set(ByVal value As DataTable)
            Session("dtNoProc") = value
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
            
            Dim scriptManager As ScriptManager = ScriptManager.GetCurrent(Me.Page)
            scriptManager.RegisterPostBackControl(Me.btn_Excel)
            
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
            'Master.cod_usuario = "AMARTINEZ"
            ValidaUsrFiltros()
        Catch ex As Exception
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "OrdenPago_FirmasElectronicas_Load: " & ex.Message)
        End Try
    End Sub
    Private Sub ValidaUsrFiltros()
        Dim ws As New ws_Generales.GeneralesClient

        Dim FiltrosUsuStr As String
        Dim FiltroFechaTope As String

        FiltrosUsuStr = ws.ObtieneParametro(Cons.TargetFiltrosAdminStro)
        FiltroFechaTope = ws.ObtieneParametro(Cons.FechaFiltroStrosTec) 'Obtiene fecha tope

        If InStr(FiltrosUsuStr, Master.cod_usuario) Then
            'If Master.cod_usuario = "CLOPEZ" Or Master.cod_usuario = "AMEZA" Or Master.cod_usuario = "CREYES" Or Master.cod_usuario = "MMQUINTERO" Then
            chk_Todas.Visible = True
            chk_Rechazadas.Visible = True
            lblFechaTope.Visible = False
        Else
            chk_Todas.Visible = True
            Dim RolUsu As String = fn_EjecutaStr("mis_ObtieneUsuxRolStro " & Master.cod_usuario)
            If RolUsu = "S" Then
                chk_MisPend.Visible = True
                chk_Rechazadas.Visible = True
                chk_FinalAut.Visible = True

            Else
                chk_MisPend.Visible = True
                chk_SinFirma.Visible = True
                chk_Rechazadas.Visible = True
                chk_FinalAut.Visible = True
            End If
            lblFechaTope.Visible = True
            lblFechaTope.Text = "Fecha hist&oacute;rica tope: " & FiltroFechaTope
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
            '''sFiltroUsuario = Funciones.ObtieneElementos(gvd_Usuario, "Usu", True)

            '''sFiltroUsuario = IIf(Not String.IsNullOrWhiteSpace(sFiltroUsuario), String.Format("AND t.cod_usuario IN ('{0}')", sFiltroUsuario), String.Empty)


            If IsDate(txtFechaGeneracionDesde.Text.Trim) And IsDate(txtFechaGeneracionHasta.Text.Trim) Then
                If CDate(txtFechaGeneracionDesde.Text.Trim) <= CDate(txtFechaGeneracionHasta.Text.Trim) Then
                    sFiltroFechaGeneracion = String.Format(" AND CONVERT(VARCHAR(10),fec_generacion,112) >= ''{0}'' AND CONVERT(VARCHAR(10),fec_generacion,112) <= ''{1}'' ", CDate(txtFechaGeneracionDesde.Text).ToString("yyyyMMdd"), CDate(txtFechaGeneracionHasta.Text).ToString("yyyyMMdd"))
                End If
            End If

            If IsDate(txtFechaPagoDesde.Text) And IsDate(txtFechaPagoHasta.Text) Then
                If CDate(txtFechaPagoDesde.Text) <= CDate(txtFechaPagoHasta.Text) Then
                    sFiltroFechaPago = String.Format(" AND CONVERT(VARCHAR(10),mop.fec_estim_pago,112) >= ''{0}'' AND CONVERT(VARCHAR(10),mop.fec_estim_pago,112) <= ''{1}'' ", CDate(txtFechaPagoDesde.Text).ToString("yyyyMMdd"), CDate(txtFechaPagoHasta.Text).ToString("yyyyMMdd"))
                End If
            End If



            Dim ws1 As New ws_Generales.GeneralesClient

            Dim FechaTope As String = ws1.ObtieneParametro(Cons.FechaFiltroStrosTec) 'Obtiene fecha tope
            Dim FiltroUsuStr As String = ws1.ObtieneParametro(Cons.TargetFiltrosAdminStro) 'Obtiene usuarios admin
            Dim FechaStop As Date

            If InStr(1, FiltroUsuStr, Master.cod_usuario) = 0 Then  'si no es parte de administracion de siniestros

                FechaStop = CDate(FechaTope)
                If IsDate(txtFechaGeneracionDesde.Text.Trim) And IsDate(txtFechaGeneracionHasta.Text.Trim) Then
                    If CDate(txtFechaGeneracionDesde.Text.Trim) < FechaStop Then
                        Mensaje.MuestraMensaje("Validacion filtro de Fecha", "La fecha de generación elegida, no puede ser menor a: " & FechaTope, TipoMsg.Advertencia)
                        Exit Function
                    End If
                End If
                If IsDate(txtFechaPagoDesde.Text) And IsDate(txtFechaPagoHasta.Text) Then
                    If CDate(txtFechaPagoDesde.Text.Trim) < FechaStop Then
                        Mensaje.MuestraMensaje("Validacion filtro de Fecha", "La fecha de pago elegida, no puede ser menor a: " & FechaTope, TipoMsg.Advertencia)
                        Exit Function
                    End If
                End If
            End If
            'If IsDate(fecFilter_De.Text) And IsDate(fecFilter_Hasta.Text) Then
            '    If CDate(fecFilter_De.Text) <= CDate(fecFilter_Hasta.Text) Then
            '        sFiltroFecDe = CDate(fecFilter_De.Text).ToString("yyyy-MM-dd")
            '        sFiltroFecHasta = CDate(fecFilter_Hasta.Text).ToString("yyyy-MM-dd")
            '    End If
            'End If

            If IsNumeric(txtMontoDesde.Text.Trim) Then
                sFiltroMonto = String.Format(" AND mop.imp_total >= {0}", txtMontoDesde.Text.Trim)
            End If

            If IsNumeric(txtMontoHasta.Text.Trim) Then
                sFiltroMonto = String.Format("{0} AND mop.imp_total <= {1}", sFiltroMonto, txtMontoHasta.Text.Trim)
            End If

            If chk_Todas.Checked Then
                iStatusFirma = Cons.TipoFiltroN.Todas
            ElseIf chk_MisPend.Checked Then
                iStatusFirma = Cons.TipoFiltroN.Pendientes
            ElseIf chk_SinFirma.Checked Then
                iStatusFirma = Cons.TipoFiltroN.SinFirma
            ElseIf chk_Rechazadas.Checked Then
                iStatusFirma = Cons.TipoFiltroN.Rechazadas
            ElseIf chk_FinalAut.Checked Then
                iStatusFirma = Cons.TipoFiltroN.Autorizadas
            End If

            Dim valorMoneda As String = ""
            If cmbMoneda.SelectedItem.Text <> ". . ." Then valorMoneda = cmbMoneda.SelectedItem.Text

            If txtSiniestro.Text <> "" Then sFiltroStro = txtSiniestro.Text
            If txtAsegurado.Text <> "" Then sFiltroBenef = txtAsegurado.Text


            Dim ValorRol As Integer = 0
            'ValorRol = ddlRolFilter.SelectedValue

            '' exportacion a excel----
            'Dim strExcel As String = String.Format("usp_ObtenerOrdenPago_stro_T_ALT {0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}'",
            '                                  Cons.StrosTradicional,
            '                                  sFiltroOP,
            '                                  sFiltroMonto,
            '                                  sFiltroFechaGeneracion,
            '                                  sFiltroFechaPago,
            '                                  sFiltroUsuario,
            '                                  Master.cod_usuario,
            '                                  iStatusFirma,
            '                                  ValorRol,
            '                                   valorMoneda,
            '                                    sFiltroStro,
            '                                    sFiltroBenef,
            '                                    sFiltroFecDe,
            '                                    sFiltroFecHasta)

            'strExcel = Replace(strExcel, "'", "''")
            'strExcel = strExcel.Substring(1, Len(strExcel) - 1)
            'strExcel = "'u" & strExcel & "'"

            'Dim resultadoExcel = fn_Ejecuta("mis_UpdParamStrExcel " & strExcel)
            '---------

            'Cambiar SP por original (usp_ObtenerOrdenPago_stro)
            fn_Consulta(String.Format("usp_ObtenerOrdenPago_stro_T_ALT '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}','{13}'",
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

                'gvd_Usuario.Enabled = False
                'btn_AddUsuario.Visible = False
                'gvd_Estatus.Enabled = False
                'btn_AddEstatus.Visible = False
                BarraEstados(False)
                'txtSiniestro.Enabled = False
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

                chk_MisPend.Enabled = False
                chk_FinalAut.Enabled = False

                'chk_Solicitante.Enabled = False
                'chk_JefeDirecto.Enabled = False
                'chk_SubDirector.Enabled = False
                'chk_Director.Enabled = False
                'chk_DirectorGral.Enabled = False
                'chk_Tesoreria.Enabled = False
                'chk_Contabilidad.Enabled = False

                btn_BuscaOP.Visible = False

                '''btn_Todas.Visible = True
                '''btn_Ninguna.Visible = True

                '''btn_Imprimir.Visible = True
                '''btn_SelTodos.Visible = True

                'btn_Firmar.Visible = True
                'btn_Excel.Visible = True

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
                '''txtSiniestro.Enabled = True
                '''gvd_Usuario.Enabled = True
                '''btn_AddUsuario.Visible = True
                '''gvd_Usuario.DataSource = Nothing
                '''gvd_Usuario.DataBind()
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

                '''chk_Pendiente.Enabled = True
                '''chk_Autorizada.Enabled = True
                chk_MisPend.Checked = False
                chk_FinalAut.Checked = False

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

                '''btn_Todas.Visible = False
                '''btn_Ninguna.Visible = False

                '''btn_Imprimir.Visible = False
                '''btn_SelTodos.Visible = False
                btn_Firmar.Visible = False
                btn_Excel.Visible = False
                btn_BuscaOP.Visible = True
                'btn_Rechazar.Visible = False
                'btn_Limpiar_Click(Nothing, )

                hid_Ventanas.Value = "0|1|1|1|1|1|1|1|"

                hid_rechazo.Value = 0

        End Select
    End Sub
    Private Sub BarraEstados(valor As Boolean)
        chk_Todas.Enabled = valor
        chk_MisPend.Enabled = valor
        chk_SinFirma.Enabled = valor
        chk_Rechazadas.Enabled = valor
        chk_FinalAut.Enabled = valor
        'chk_NoProc.Enabled = valor
        'ddlRolFilter.Enabled = valor
    End Sub


    Private Sub LimpiaCtrls()
        txt_NroOP.Text = ""
        'cmbModuloOP.SelectedIndex = 0
        'cmbMoneda.SelectedIndex = 0
        'ddlRolFilter.SelectedIndex = 0
        txtFechaGeneracionDesde.Text = ""
        txtFechaGeneracionHasta.Text = ""
        chk_MisPend.Checked = False
        '''chk_Revisadas.Checked = False
        chk_Todas.Checked = False
        'chk_NoProc.Checked = False
        chk_SinFirma.Checked = False
        '''chk_Pendiente.Checked = False
        chk_Rechazadas.Checked = False
        chk_FinalAut.Checked = False
        'ddlRolFilter.Visible = False
        txtMontoDesde.Text = ""
        txtMontoHasta.Text = ""
        txtFechaPagoDesde.Text = ""
        txtFechaPagoHasta.Text = ""
        'txtSiniestro.Text = ""
        txtAsegurado.Text = ""
        'fecFilter_De.Text = ""
        'fecFilter_Hasta.Text = ""



    End Sub

    Private Sub btn_BuscaOP_Click(sender As Object, e As EventArgs) Handles btn_BuscaOP.Click
        Try
            'If cmbModuloOP.SelectedValue > 0 Then
            If ValidaRadios() Then
                'Funciones.LlenaGrid(grdOrdenPago, ConsultaOrdenesPagoSiniestros(cmbModuloOP.SelectedValue))
                Funciones.LlenaGrid(grdOrdenPago, ConsultaOrdenesPagoSiniestros(Cons.StrosTradicional))

                If grdOrdenPago.Rows.Count > 0 Then
                    grdOrdenPago.PageIndex = 0
                    btn_Firmar.Visible = True
                    btn_Excel.Visible = True
                    EdoControl(Operacion.Consulta)
                    MuestraChecksAccion()

                    'For Each row In dtOrdenPago.Rows

                    '    Dim ddl_ConceptoRech = DirectCast(grdOrdenPago.FindControl("cmbConcepto"), DropDownList)

                    '    Dim dtDll As DataTable = Nothing
                    '    Funciones.fn_Consulta("mis_ConceptosAnulacion", dtDll)
                    '    ddl_ConceptoRech.DataSource = dtDll
                    '    ddl_ConceptoRech.DataBind()

                    'Next

                    ' DesHabilitaChecksFirma()
                    Funciones.EjecutaFuncion("fn_EstadoFilas('grdOrdenPago',true);")
                Else
                    btn_Firmar.Visible = False
                    btn_Excel.Visible = False
                    Mensaje.MuestraMensaje("Autorizaciones Electr&oacute;nicas", "La Consulta no devolvi&oacute; resultados", TipoMsg.Advertencia)
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
            'If chk_NoProc.Checked = False Then
            If chk_MisPend.Checked = False Then
                If chk_SinFirma.Checked = False Then
                    If chk_Rechazadas.Checked = False Then
                        If chk_FinalAut.Checked = False Then
                            ValidaRadios = False
                        End If
                    End If
                End If
            End If
            'End If
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

    'Private Sub btn_Firmar_Click(sender As Object, e As EventArgs) Handles btn_Firmar.Click
    '    Try
    '        ' ActualizaDataOP()
    '        'txtToken.Text = ""


    '        'fn_Token()
    '        If fn_Autorizaciones(False) = False Then
    '            Exit Sub
    '        End If

    '        'hid_rechazo.Value = 0
    '        'Master.Titulo_Autoriza = "AUTORIZACIÓN ORDENES DE PAGO"
    '        'Funciones.EjecutaFuncion("fn_Autoriza();")
    '    Catch ex As Exception
    '        Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
    '        Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Firmar_Click: " & ex.Message)
    '    End Try
    'End Sub


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
        dtAutorizaciones.Columns.Add("NombreModifica")
        'Codigos
        dtAutorizaciones.Columns.Add("Solicitante")
        dtAutorizaciones.Columns.Add("Jefe")
        dtAutorizaciones.Columns.Add("Subgerente")
        dtAutorizaciones.Columns.Add("Gerente")
        dtAutorizaciones.Columns.Add("SubDirector")
        dtAutorizaciones.Columns.Add("Director")
        dtAutorizaciones.Columns.Add("DirectorEjecutivo")
        dtAutorizaciones.Columns.Add("DirectorGeneral")
        dtAutorizaciones.Columns.Add("Tesoreria")

        'Switch Banderas
        dtAutorizaciones.Columns.Add("FirmadoSolicitante")
        dtAutorizaciones.Columns.Add("FirmadoJefe")
        dtAutorizaciones.Columns.Add("FirmadoSubgerente")
        dtAutorizaciones.Columns.Add("FirmadoGerente")
        dtAutorizaciones.Columns.Add("FirmadoSubdirector")
        dtAutorizaciones.Columns.Add("FirmadoDirector")
        dtAutorizaciones.Columns.Add("FirmadoDirectorEjecutivo")
        dtAutorizaciones.Columns.Add("FirmadoDirectorGeneral")
        dtAutorizaciones.Columns.Add("FirmadoTesoreria")




        For Each row In dtOrdenPago.Rows

            dtAutorizaciones.Rows.Add(row("nro_op"), Master.cod_usuario, Master.user, Master.usuario,
                                      IIf(IsDBNull(row("RoleUsuario")) = vbTrue, "C", row("RoleUsuario")),
                                        row("NivelAutorizacion"),
                                        row("NombreModifica"),
                                        row("Solicitante"),
                                        row("Jefe"),
                                        row("Subgerente"),
                                        row("Gerente"),
                                        row("SubDirector"),
                                        row("Director"),
                                        row("DirectorEjecutivo"),
                                        row("DirectorGeneral"),
                                        row("Tesoreria"),
                                        row("FirmadoSolicitante"),
                                        row("FirmadoJefe"),
                                        row("FirmadoSubgerente"),
                                        row("FirmadoGerente"),
                                        row("FirmadoSubdirector"),
                                        row("FirmadoDirector"),
                                        row("FirmadoDirectorEjecutivo"),
                                        row("FirmadoDirectorGeneral"),
                                        row("FirmadoTesoreria"))
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

        dtSegRev = New DataTable
        dtSegRev.Columns.Add("noOP")

        dtNoProc = New DataTable
        dtNoProc.Columns.Add("noOP")
        dtNoProc.Columns.Add("Justificacion")

        'Dim numPaginas As Integer
        'Dim paginaActual As Integer
        'Dim RowsMostrados As Integer

        'numPaginas = grdOrdenPago.PageCount
        'paginaActual = grdOrdenPago.PageIndex
        'RowsMostrados = grdOrdenPago.Rows.Count
        'Dim RegTotales As Integer = dtAutorizaciones.Rows.Count

        'Dim contadorIni As Integer = 0
        'Select Case grdOrdenPago.PageIndex
        '    Case 0
        '        contador = 0
        '        contadorIni = 0
        '    Case 1
        '        contador = 0
        '        contadorIni = 17
        '    Case 2
        '        contador = 0
        '        contadorIni = 37
        '    Case 3
        '        contador = 0
        '        contadorIni = 57
        '    Case 4
        '        contador = 0 
        '        contadorIni = 77
        'End Select

        For Each row In dtAutorizaciones.Rows


            'If contadorIni = dtAutorizaciones.Rows.IndexOf(row) And contador <= (RowsMostrados - 1) Then


            Dim chk_Firmar_ As Boolean = DirectCast(grdOrdenPago.Rows(contador).FindControl("chkFirmar"), CheckBox).Checked
            Dim chk_SegRev_ As Boolean = DirectCast(grdOrdenPago.Rows(contador).FindControl("chkSegRev"), CheckBox).Checked
            Dim chk_NoProc_ As Boolean = DirectCast(grdOrdenPago.Rows(contador).FindControl("chkNoProc"), CheckBox).Checked
            Dim chk_Rechazo As Boolean = DirectCast(grdOrdenPago.Rows(contador).FindControl("chkSolRechazo"), CheckBox).Checked
            Dim chk_Revision As Boolean = DirectCast(grdOrdenPago.Rows(contador).FindControl("chkRevision"), CheckBox).Checked
            Dim ddlMotivo = DirectCast(grdOrdenPago.Rows(contador).FindControl("cmbConcepto"), DropDownList)
            Dim txtMotivoNoProc_ = DirectCast(grdOrdenPago.Rows(contador).FindControl("txtMotivoNoProc"), TextBox).Text

            Dim txtJustif As String = DirectCast(grdOrdenPago.Rows(contador).FindControl("cmbConcepto"), DropDownList).SelectedItem.Text
            Dim txtOtros As String = DirectCast(grdOrdenPago.Rows(contador).FindControl("txtMotivoOtro"), TextBox).Text

            If ddlMotivo.SelectedValue = 11 Then
                txtJustif = txtOtros
            End If

            strOP = row("nro_op")


            'If chk_Impresion = True Then
            If chk_Firmar_ = True Then

                Dim Rechazada As Integer = fn_Ejecuta("mis_ValidaStsOp " & strOP)
                If Rechazada = 1 Then
                    Mensaje.MuestraMensaje("Validación", "la Orden de Pago: " & strOP & " ya se encuentra rechazada, por favor deseleccionarla", TipoMsg.Advertencia)
                    Exit Function
                End If

                'Evalua si esta firmado por el analista
                If codRol <> "S" Then
                    Dim ResultSol As Integer = fn_Ejecuta("mis_VerFirmaSolicitante " & strOP)
                    If ResultSol = 0 Then
                        Mensaje.MuestraMensaje("Validación", "la Orden de Pago: " & strOP & " requiere la firma del analista, por favor dejarla En Revision", TipoMsg.Advertencia)
                        Exit Function
                    End If
                End If

                If row("NivelAutorizacion") = 1 Then
                    If DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                        UsuarioFirma = row("Solicitante")
                    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                        UsuarioFirma = row("Jefe")
                        OPCompletada = True
                    End If
                    If codRol = "J" Or codRol = "SG" Or codRol = "G" Or codRol = "SD" Or codRol = "D" Or codRol = "DE" Or codRol = "DG" Then OPCompletada = True

                ElseIf row("NivelAutorizacion") = 2 Then
                        If DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Solicitante")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Jefe")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Subgerente")
                            OPCompletada = True
                        End If
                    If codRol = "SG" Or codRol = "G" Or codRol = "SD" Or codRol = "D" Or codRol = "DE" Or codRol = "DG" Then OPCompletada = True
                ElseIf row("NivelAutorizacion") = 3 Then
                        If DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Solicitante")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Jefe")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Subgerente")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Gerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Gerente")
                            OPCompletada = True
                        End If
                    If codRol = "G" Or codRol = "SD" Or codRol = "D" Or codRol = "DE" Or codRol = "DG" Then OPCompletada = True
                ElseIf row("NivelAutorizacion") = 4 Then
                    If DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                        UsuarioFirma = row("Solicitante")
                    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                        UsuarioFirma = row("Jefe")
                    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                        UsuarioFirma = row("Subgerente")
                    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Gerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                        UsuarioFirma = row("Gerente")
                    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subdirector_"), Label).BackColor = System.Drawing.Color.Orange Then
                        UsuarioFirma = row("Subdirector")
                        OPCompletada = True
                    End If
                    If codRol = "SD" Or codRol = "D" Or codRol = "DE" Or codRol = "DG" Then OPCompletada = True

                ElseIf row("NivelAutorizacion") = 5 Then
                        If DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Solicitante")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Jefe")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Subgerente")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Gerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Gerente")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subdirector_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Subdirector")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Director_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Director")
                            OPCompletada = True
                        End If
                    If codRol = "D" Or codRol = "DE" Or codRol = "DG" Then OPCompletada = True
                ElseIf row("NivelAutorizacion") = 6 Then
                        If DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Solicitante")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Jefe")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Subgerente")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Gerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Gerente")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subdirector_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Subdirector")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Director_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Director")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("DirectorEjecutivo_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("DirectorEjecutivo")
                            OPCompletada = True
                        End If
                    If codRol = "DE" Or codRol = "DG" Then OPCompletada = True
                ElseIf row("NivelAutorizacion") = 7 Then
                        If DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Solicitante")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Jefe")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Subgerente")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Gerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Gerente")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subdirector_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Subdirector")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Director_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("Director")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("DirectorEjecutivo_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("DirectorEjecutivo")
                        ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("DirectorGeneral_"), Label).BackColor = System.Drawing.Color.Orange Then
                            UsuarioFirma = row("DirectorGeneral")
                            OPCompletada = True
                        End If

                End If
                If OPCompletada = False Then

                    If sn_proceso = True Then
                        If fn_Ejecuta("usp_AplicaFirmasOP_stro " & strOP & ",-1,'" & codRol & "'," & 1) = 1 Then
                            'fn_Ejecuta("mis_InsertaOPsEnviadas " & strOP & ",'" & UsuarioFirma & "'," & Cons.StrosTradicional & ",0")
                            If row("NivelAutorizacion") >= 6 Then
                                fn_Ejecuta("mis_EmailsOPStros '" & strOP & "','" & Cons.StrosTradicional & "','" & UsuarioFirma & "','" & Master.usuario & "','" & codRol & "'")
                            End If
                            Mensaje.MuestraMensaje("Autorizaciones", "Se aplicaron las firmas correspondientes", Mensaje.TipoMsg.Confirma)
                        End If
                        '''End If
                    End If
                Else
                    If sn_proceso = True Then
                        If fn_Ejecuta("usp_AplicaFirmasOP_stro " & strOP & ",-1,'" & codRol & "'," & 1) = 1 Then
                            fn_Ejecuta("mis_AutorizaOPSTros " & strOP & ",'" & Master.cod_usuario & "'")
                            OPCompletada = False
                            Mensaje.MuestraMensaje("Autorizaciones", "Se Autorizo la Orden de Pago", Mensaje.TipoMsg.Confirma)
                        End If
                    End If
                End If
                dtAutoriza.Rows.Add(strOP)

            ElseIf chk_Rechazo = True Then 'rechazo
                    Dim codMotivoRechazo As Integer
                    Dim strMotivoRechazo As String
                    Dim intFolioOnBase As Integer
                    codMotivoRechazo = DirectCast(grdOrdenPago.Rows(contador).FindControl("cmbConcepto"), DropDownList).SelectedItem.Value
                    strMotivoRechazo = DirectCast(grdOrdenPago.Rows(contador).FindControl("cmbConcepto"), DropDownList).SelectedItem.Text
                intFolioOnBase = DirectCast(grdOrdenPago.Rows(contador).FindControl("folioonbase"), Label).Text

                If codMotivoRechazo = 11 Then
                    strMotivoRechazo = txtOtros
                End If

                Dim Rechazada As Integer = fn_Ejecuta("mis_ValidaStsOp " & strOP)
                    If Rechazada = 1 Then
                        Mensaje.MuestraMensaje("Validación Rechazos", "la Orden de Pago: " & strOP & " ya se encuentra rechazada, por favor deseleccionarla", TipoMsg.Advertencia)
                        Exit Function
                    End If

                    If strMotivoRechazo = "--Seleccione--" Then
                        Mensaje.MuestraMensaje("Validación Motivo de Rechazo", "No se ha seleccionado ningun motivo de rechazo de para la OP: " & strOP, TipoMsg.Advertencia)
                        Exit Function
                    End If



                    'If ddlMotivo.SelectedValue = 11 Then
                    '    strMotivoRechazo = txtOtros

                    'End If

                    If sn_proceso = True Then


                    '''If row("NivelAutorizacion") = 1 Then

                    '''    If DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Jefe")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")

                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Solicitante")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    End If

                    '''ElseIf row("NivelAutorizacion") = 2 Then

                    '''    If DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Subgerente")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Jefe")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Solicitante")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    End If

                    '''ElseIf row("NivelAutorizacion") = 3 Then

                    '''    If DirectCast(grdOrdenPago.Rows(contador).FindControl("Gerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Gerente")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Subgerente")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Jefe")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Solicitante")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    End If

                    '''ElseIf row("NivelAutorizacion") = 4 Then

                    '''    If DirectCast(grdOrdenPago.Rows(contador).FindControl("Subdirector_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Subdirector")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Gerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Gerente")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Subgerente")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Jefe")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Solicitante")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    End If

                    '''ElseIf row("NivelAutorizacion") = 5 Then
                    '''    If DirectCast(grdOrdenPago.Rows(contador).FindControl("Director_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Director")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subdirector_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Subdirector")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Gerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Gerente")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Subgerente")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Jefe")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Solicitante")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    End If

                    '''ElseIf row("NivelAutorizacion") = 6 Then
                    '''    If DirectCast(grdOrdenPago.Rows(contador).FindControl("DirectorEjecutivo_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("DirectorEjecutivo")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Director_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Director")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subdirector_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Subdirector")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Gerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Gerente")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Subgerente")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Jefe")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Solicitante")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    End If

                    '''ElseIf row("NivelAutorizacion") = 7 Then

                    '''    If DirectCast(grdOrdenPago.Rows(contador).FindControl("DirectorGeneral_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("DirectorGeneral")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("DirectorEjecutivo_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("DirectorEjecutivo")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Director_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Director")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subdirector_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Subdirector")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Subgerente_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Subgerente")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Jefe_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Jefe")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    ElseIf DirectCast(grdOrdenPago.Rows(contador).FindControl("Solicitante_"), Label).BackColor = System.Drawing.Color.Orange Then
                    '''        UsuarioFirma = row("Solicitante")
                    '''        fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '''    End If

                    '''End If
                    Select Case codRol
                        Case "S"
                            UsuarioFirma = row("Solicitante")
                        Case "J"
                            UsuarioFirma = row("Jefe")
                        Case "SG"
                            UsuarioFirma = row("Subgerente")
                        Case "G"
                            UsuarioFirma = row("Gerente")
                        Case "SD"
                            UsuarioFirma = row("Subdirector")
                        Case "D"
                            UsuarioFirma = row("Director")
                        Case "DE"
                            UsuarioFirma = row("DirectorEjecutivo")
                        Case "DG"
                            UsuarioFirma = row("DirectorGeneral")
                    End Select
                    'Mail a quien solicita el rechazo
                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & UsuarioFirma & "','" & Master.usuario & "','" & strMotivoRechazo & "'")

                    'Aplica solicitud de rechazo en mop_firmas
                    fn_Ejecuta("usp_AplicaFirmasOP_stro " & strOP & ",0,'" & codRol & "',0,'Usuario: " & Master.usuario & " /Motivo: " & strMotivoRechazo & "','" & Master.cod_usuario & "'")

                    'Envia mail a pagos_siniestros y a quien elaboro la op
                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','CLOPEZ','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & row("NombreModifica") & "','" & Master.usuario & "','" & strMotivoRechazo & "'")

                    ''Aqui corria la cancelacion. se comenta por 2da fase
                    'fn_Ejecuta("usp_AplicaFirmasOP_stro " & strOP & ",0,'" & codRol & "',0,'Usuario: " & Master.usuario & " /Motivo: " & strMotivoRechazo & "','" & Master.cod_usuario & "'")

                    'fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','CLOPEZ','" & Master.usuario & "','" & strMotivoRechazo & "'")
                    '    fn_Ejecuta("mis_MailPeticionRechazo '" & strOP & "','" & row("NombreModifica") & "','" & Master.usuario & "','" & strMotivoRechazo & "'")
                Else
                        dtCancela.Rows.Add(strOP, txtJustif)
                    End If

                ElseIf chk_SegRev_ = True Then
                    If sn_proceso = True Then

                        '
                        Select Case Master.cod_usuario

                            Case row("DirectorGeneral")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("DirectorEjecutivo") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Director") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Subdirector") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Gerente") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Subgerente") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Jefe") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Solicitante") & "'")

                            Case row("DirectorEjecutivo")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Director") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Subdirector") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Gerente") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Subgerente") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Jefe") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Solicitante") & "'")

                            Case row("Director")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Subdirector") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Gerente") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Subgerente") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Jefe") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Solicitante") & "'")

                            Case row("Subdirector")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Gerente") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Subgerente") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Jefe") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Solicitante") & "'")

                            Case row("Gerente")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Subgerente") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Jefe") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Solicitante") & "'")

                            Case row("Subgerente")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Jefe") & "'")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Solicitante") & "'")

                            Case row("Jefe")
                                fn_Ejecuta("mis_MailSegundaRev '" & strOP & "','" & Master.usuario & "','" & row("Solicitante") & "'")

                        End Select


                        Mensaje.MuestraMensaje("Proceso Autorizacion Electrónica", "Se envia a segunda revisión la OP", TipoMsg.Confirma)
                    Else
                        dtSegRev.Rows.Add(strOP)
                    End If

                ElseIf chk_NoProc_ = True Then
                    If sn_proceso = True Then
                        fn_Ejecuta("mis_RechazoNoProc '" & strOP & "','" & txtMotivoNoProc_ & "','" & Master.cod_usuario & "'")
                        fn_Ejecuta("mis_MailRechazoNoProc '" & strOP & "','" & Master.usuario & "','" & txtMotivoNoProc_ & "'")
                        Mensaje.MuestraMensaje("Proceso Autorizacion Electrónica", "Se rechazo la solicitud de cancelación", TipoMsg.Confirma)
                    Else

                        If txtMotivoNoProc_ = "" Then
                            Mensaje.MuestraMensaje("Validación Motivo no Procedente", "No se ha indicado ningun motivo por el que no procede el rechazo de la OP: " & strOP, TipoMsg.Advertencia)
                            Exit Function
                        End If

                        dtNoProc.Rows.Add(strOP, txtMotivoNoProc_)
                    End If
                ElseIf chk_Revision = True Then


                End If

            'End If
            contador = contador + 1
            '    contadorIni = contadorIni + 1
            'Else
            '    'contador = contador + 1
            'End If


        Next

        gvd_Canceladas.DataSource = dtCancela
        gvd_Canceladas.DataBind()
        gvd_Autorizadas.DataSource = dtAutoriza
        gvd_Autorizadas.DataBind()

        gvd_segrev.DataSource = dtSegRev
        gvd_segrev.DataBind()
        gvd_noproc.DataSource = dtNoProc
        gvd_noproc.DataBind()


        If dtCancela Is Nothing And dtAutoriza Is Nothing And dtSegRev Is Nothing And dtNoProc Is Nothing Then
            Mensaje.MuestraMensaje(Master.Titulo, "No se ha seleccionado ninguna Orden de Pago para alguna accion", TipoMsg.Advertencia)
        Else

            gvd_Canceladas.DataSource = dtCancela
            gvd_Canceladas.DataBind()
            gvd_Autorizadas.DataSource = dtAutoriza
            gvd_Autorizadas.DataBind()

            gvd_segrev.DataSource = dtSegRev
            gvd_segrev.DataBind()
            gvd_noproc.DataSource = dtNoProc
            gvd_noproc.DataBind()

            If sn_proceso = True Then
                'If Master.cod_usuario <> "CLOPEZ" Then

                '    Funciones.fn_Consulta("mis_ObtieneOpEnvioFirma 0,''," & Cons.StrosTradicional, dtEnvios)

                '    For Each item In dtEnvios.Rows
                '        fn_Ejecuta("mis_EmailsOPStros '" & item("Ops") & "','" & item("tipomodulo") & "','" & item("cod_usuario") & "','" & Master.usuario & "','" & codRol & "'")
                '        fn_Ejecuta("mis_ActualizaStsOpsEnv '" & item("Ops") & "','" & item("cod_usuario") & "'," & Cons.StrosTradicional & ",1")
                '    Next
                '''fn_Ejecuta("mis_EmailsOPStros '" & strOP & "','" & Cons.StrosTradicional & "','" & UsuarioFirma & "','" & Master.usuario & "','" & codRol & "'")
                '''Mensaje.MuestraMensaje("Autorizaciones", "Se realizaron las acciones correctamente", Mensaje.TipoMsg.Confirma)

                'End If

                btn_Limpiar_Click(Nothing, Nothing)

            End If
        End If
        fn_Autorizaciones = True

        If sn_proceso = False Then
            If gvd_Canceladas.Rows.Count > 0 Or gvd_Autorizadas.Rows.Count > 0 Or gvd_segrev.Rows.Count > 0 Then
                txtToken.Text = ""
                txtToken.Visible = True
                lblToken.Visible = True
                TitCancel.Visible = True
                pnlCancelar.Visible = True
                TitAutoriza.Visible = True
                pnlAutoriza.Visible = True
                TitNoProc.Visible = False
                pnlNoProc.Visible = False
                TituloSegRev.Visible = True
                pnlSegRev.Visible = True
                fn_Token()
            Else 'no proceden
                TitNoProc.Visible = True
                pnlNoProc.Visible = True
                TituloSegRev.Visible = False
                pnlSegRev.Visible = False
                TitCancel.Visible = False
                pnlCancelar.Visible = False
                TitAutoriza.Visible = False
                pnlAutoriza.Visible = False

                lblToken.Visible = False
                txtToken.Visible = False
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

    Private Sub MuestraChecksAccion()

        grdOrdenPago.Columns(21).Visible = False 'oculta el txtMotivoOtro


        If chk_MisPend.Visible = False Then 'valida si es Administracion de usuarios
            If chk_Todas.Checked Then       ' TODAS ADMIN
                grdOrdenPago.Columns(17).Visible = False
                grdOrdenPago.Columns(18).Visible = False
                grdOrdenPago.Columns(19).Visible = False
                grdOrdenPago.Columns(20).Visible = False
                grdOrdenPago.Columns(21).Visible = False
                grdOrdenPago.Columns(22).Visible = False
                grdOrdenPago.Columns(23).Visible = False
                grdOrdenPago.Columns(24).Visible = False

            ElseIf chk_Rechazadas.Checked Then     'RECHAZADAS
                grdOrdenPago.Columns(17).Visible = False
                grdOrdenPago.Columns(18).Visible = False
                grdOrdenPago.Columns(19).Visible = False
                grdOrdenPago.Columns(20).Visible = False
                grdOrdenPago.Columns(21).Visible = False
                grdOrdenPago.Columns(23).Visible = False

                grdOrdenPago.Columns(22).Visible = True
                grdOrdenPago.Columns(24).Visible = True

            End If

        ElseIf chk_Todas.Checked Then        'TODAS TECNICO
            grdOrdenPago.Columns(17).Visible = False
            grdOrdenPago.Columns(18).Visible = False
            grdOrdenPago.Columns(19).Visible = False
            grdOrdenPago.Columns(20).Visible = False
            grdOrdenPago.Columns(21).Visible = False
            grdOrdenPago.Columns(22).Visible = False
            grdOrdenPago.Columns(23).Visible = False
            grdOrdenPago.Columns(24).Visible = False

        ElseIf chk_MisPend.Checked             'MIS PENDIENTES
            grdOrdenPago.Columns(22).Visible = False
            grdOrdenPago.Columns(24).Visible = False
            grdOrdenPago.Columns(26).Visible = False

            grdOrdenPago.Columns(17).Visible = True
            grdOrdenPago.Columns(18).Visible = True
            grdOrdenPago.Columns(19).Visible = True
            grdOrdenPago.Columns(20).Visible = True
            grdOrdenPago.Columns(21).Visible = True
            grdOrdenPago.Columns(23).Visible = True


        ElseIf chk_SinFirma.Checked            'SIN FIRMA PREVIA  Y FIRMA EN AUSENCIA
            grdOrdenPago.Columns(22).Visible = False
            grdOrdenPago.Columns(24).Visible = False
            grdOrdenPago.Columns(26).Visible = False

            grdOrdenPago.Columns(17).Visible = True
            grdOrdenPago.Columns(18).Visible = True
            grdOrdenPago.Columns(19).Visible = True
            grdOrdenPago.Columns(20).Visible = True
            grdOrdenPago.Columns(21).Visible = True
            grdOrdenPago.Columns(23).Visible = True

        ElseIf chk_Rechazadas.Checked Then     'RECHAZADAS TECNICO
            grdOrdenPago.Columns(17).Visible = False
            grdOrdenPago.Columns(18).Visible = False
            grdOrdenPago.Columns(19).Visible = False
            grdOrdenPago.Columns(20).Visible = False
            grdOrdenPago.Columns(21).Visible = False
            grdOrdenPago.Columns(23).Visible = False
            grdOrdenPago.Columns(26).Visible = True

        ElseIf chk_FinalAut.Checked Then      'AUTORIZADAS
            grdOrdenPago.Columns(17).Visible = False
            grdOrdenPago.Columns(18).Visible = False
            grdOrdenPago.Columns(19).Visible = False
            grdOrdenPago.Columns(20).Visible = False
            grdOrdenPago.Columns(21).Visible = False
            grdOrdenPago.Columns(22).Visible = False
            grdOrdenPago.Columns(23).Visible = False
            grdOrdenPago.Columns(24).Visible = False


        End If

    End Sub
    'Private Sub MuestraChecksAccion()
    '    grdOrdenPago.Columns(20).Visible = False 'oculta el txtMotivoOtro
    '    If chk_Rechazadas.Checked = True And chk_MisPend.Visible = False Then  'si Administracion de siniestros
    '        grdOrdenPago.Columns(21).Visible = True 'no procede
    '        grdOrdenPago.Columns(23).Visible = True 'motivo

    '        ' grdOrdenPago.Columns(15).Visible = False
    '        grdOrdenPago.Columns(16).Visible = False
    '        grdOrdenPago.Columns(17).Visible = False
    '        grdOrdenPago.Columns(18).Visible = False
    '        grdOrdenPago.Columns(19).Visible = False
    '        'grdOrdenPago.Columns(19).Visible = False
    '        grdOrdenPago.Columns(25).Visible = True

    '    ElseIf chk_MisPend.Checked = False Then

    '        If chk_Todas.Checked = True Then
    '            grdOrdenPago.Columns(16).Visible = False
    '            grdOrdenPago.Columns(17).Visible = False
    '            grdOrdenPago.Columns(18).Visible = False
    '            grdOrdenPago.Columns(19).Visible = False
    '        Else
    '            If chk_FinalAut.Checked = True Then

    '                grdOrdenPago.Columns(16).Visible = False
    '                grdOrdenPago.Columns(17).Visible = False
    '                grdOrdenPago.Columns(18).Visible = False
    '                grdOrdenPago.Columns(19).Visible = False
    '            ElseIf chk_Rechazadas.Checked = True Then

    '                grdOrdenPago.Columns(16).Visible = False
    '                grdOrdenPago.Columns(17).Visible = False
    '                grdOrdenPago.Columns(18).Visible = False
    '                grdOrdenPago.Columns(19).Visible = False
    '            Else

    '                'se muestra por Firma en Ausencia
    '                grdOrdenPago.Columns(16).Visible = True
    '                grdOrdenPago.Columns(17).Visible = True
    '                grdOrdenPago.Columns(18).Visible = True
    '                grdOrdenPago.Columns(19).Visible = True
    '            End If

    '        End If
    '        grdOrdenPago.Columns(21).Visible = False
    '        grdOrdenPago.Columns(22).Visible = False
    '            grdOrdenPago.Columns(23).Visible = False
    '            grdOrdenPago.Columns(25).Visible = True

    '        Else
    '            grdOrdenPago.Columns(16).Visible = True  'En Revision 
    '        grdOrdenPago.Columns(17).Visible = True  'Firmar 
    '        grdOrdenPago.Columns(18).Visible = True  'Rechazar 
    '        grdOrdenPago.Columns(19).Visible = True  'Motivo Rechazo
    '        grdOrdenPago.Columns(21).Visible = False 'solo oculta no proc 

    '        If chk_SinFirma.Visible = True Then  'si no es solicitante 19
    '            grdOrdenPago.Columns(22).Visible = True
    '        Else
    '            grdOrdenPago.Columns(22).Visible = False  '2da rev oculta
    '        End If

    '        grdOrdenPago.Columns(23).Visible = False 'motivo 21
    '        grdOrdenPago.Columns(25).Visible = False
    '    End If

    'End Sub
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


    'Private Sub btn_Imprimir_Click(sender As Object, e As EventArgs) Handles btn_Imprimir.Click

    '    Dim strOrdenPago As String = "-1"

    '    Dim server As String = String.Empty

    '    Try

    '        'ActualizaDataOP()

    '        Dim ws As New ws_Generales.GeneralesClient

    '        server = ws.ObtieneParametro(3)
    '        server = Replace(Replace(server, "@Reporte", "OrdenPago"), "@Formato", "PDF") & "&nro_op=@nro_op"
    '        server = Replace(server, "ReportesGMX_DESA", "ReportesOPSiniestros_DESA")
    '        server = Replace(server, "OrdenPago", "OrdenPago_stro")

    '        For Each row In grdOrdenPago.Rows

    '            If TryCast(row.FindControl("chk_Print"), CheckBox).Checked Then
    '                strOrdenPago = String.Format("{0}, {1}", strOrdenPago, DirectCast(row.FindControl("lblOrdenPago"), Label).Text.Trim)
    '            End If

    '        Next

    '        If strOrdenPago <> "-1" Then

    '            strOrdenPago = Replace(strOrdenPago, "-1,", "")

    '            Funciones.EjecutaFuncion(String.Format("fn_Imprime_OP('{0}','{1}');",
    '                                                               server,
    '                                                               strOrdenPago))

    '        End If

    '    Catch ex As Exception
    '        Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
    '        Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Imprimir_Click: " & ex.Message)
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
            Case 0 'Todas
                If chk_Todas.Checked Then
                    chk_Rechazadas.Checked = False
                    'chk_NoProc.Checked = False
                    chk_MisPend.Checked = False
                    chk_SinFirma.Checked = False
                    chk_FinalAut.Checked = False

                End If
            Case 2 'mis pendientes
                If chk_MisPend.Checked Then
                    chk_Todas.Checked = False
                    chk_Rechazadas.Checked = False
                    ' chk_NoProc.Checked = False
                    chk_SinFirma.Checked = False
                    chk_FinalAut.Checked = False

                End If
            Case 7 'Sin firma previa
                If chk_SinFirma.Checked Then
                    chk_Todas.Checked = False
                    chk_Rechazadas.Checked = False
                    ' chk_NoProc.Checked = False
                    chk_FinalAut.Checked = False
                    chk_MisPend.Checked = False
                End If
            Case 4 'rechazadas/canceladas
                If chk_Rechazadas.Checked Then
                    chk_Todas.Checked = False
                    chk_SinFirma.Checked = False
                    'chk_NoProc.Checked = False
                    chk_FinalAut.Checked = False
                    chk_MisPend.Checked = False

                End If
            Case 5 'autorizadas
                If chk_FinalAut.Checked Then
                    chk_Todas.Checked = False
                    chk_SinFirma.Checked = False
                    'chk_NoProc.Checked = False
                    chk_Rechazadas.Checked = False
                    chk_MisPend.Checked = False

                End If
                'Case 6 'no procedentes
                '    If chk_NoProc.Checked Then
                '        chk_Todas.Checked = False
                '        chk_SinFirma.Checked = False
                '        chk_FinalAut.Checked = False
                '        chk_Rechazadas.Checked = False
                '        chk_MisPend.Checked = False

                '    End If
        End Select

    End Sub

    Private Sub lnkAceptarProc_Click(sender As Object, e As EventArgs) Handles lnkAceptarProc.Click
        Try

            Dim ws As New ws_Generales.GeneralesClient

            Dim AdminUsuStr As String
            AdminUsuStr = ws.ObtieneParametro(Cons.TargetFiltrosAdminStro)

            If InStr(AdminUsuStr, Master.cod_usuario) Then
                If fn_Autorizaciones(True) = False Then
                    Exit Sub
                End If
            Else

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


    'Protected Sub cmbConcepto_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    Dim combo As DropDownList = DirectCast(sender, DropDownList)
    '    'MsgBox(combo.SelectedValue)

    '    Dim gr As GridViewRow = DirectCast(DirectCast(combo.Parent, DataControlFieldCell).Parent, GridViewRow)
    '    ' MsgBox(gr.RowIndex)

    '    Dim txtOtros = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txtMotivoOtro"), TextBox)

    '    If combo.SelectedValue = 11 Then
    '        txtOtros.Visible = True
    '        grdOrdenPago.Columns(20).Visible = True
    '    Else
    '        txtOtros.Visible = False
    '        grdOrdenPago.Columns(20).Visible = False
    '        txtOtros.Text = ""
    '    End If

    'End Sub

    Private Sub grdOrdenPago_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grdOrdenPago.RowCommand
        Try
            Dim Index As Integer = e.CommandSource.NamingContainer.RowIndex
            Dim OrdenPago = grdOrdenPago.DataKeys(Index)("nro_op")
            Dim FolioOnBase_ = grdOrdenPago.DataKeys(Index)("FolioOnbase")
            Dim CodAbona_ = grdOrdenPago.DataKeys(Index)("CodigoAbona")
            Dim FolioOnBaseEdoCta_ = grdOrdenPago.DataKeys(Index)("Folio_Onbase_est_cuenta")
            Dim ws As New ws_Generales.GeneralesClient
            Dim server As String = ws.ObtieneParametro(Cons.TargetReport)
            Dim strURLOnBase As String = ws.ObtieneParametro(Cons.RutaWebServOnBase)
            Dim RptFilters As String
            RptFilters = "&nro_op=" & OrdenPago

            If e.CommandName = "VerOP" Then

                server = Replace(Replace(server, "@Reporte", "OrdenPago"), "@Formato", "PDF")
                server = Replace(server, Cons.ReposSource, Cons.ReposReport)
                server = Replace(server, "OrdenPago", "OrdenPago_stro")
                server = server & RptFilters
                Funciones.EjecutaFuncion("window.open('" & server & "','_blank');")

            ElseIf e.CommandName = "VerEdoCta"
                Dim hrefOnBase As String
                'hrefOnBase = ws.ObtieneParametro(Cons.RutaWebServOnBase)
                If FolioOnBaseEdoCta_.ToString() = vbNullString Or FolioOnBaseEdoCta_ = 0 Then
                    Mensaje.MuestraMensaje("Estado de Cuenta", "La OP no tiene Folio OnBase Estado de Cuenta", TipoMsg.Advertencia)
                Else
                    hrefOnBase = fn_EjecutaStr("mis_WSOnBasePath " & -1)
                    hrefOnBase = Replace(hrefOnBase, "@Folio", FolioOnBaseEdoCta_)
                    Funciones.EjecutaFuncion("window.open('" & hrefOnBase & "','_blank');")
                End If
            ElseIf e.CommandName = "VerDocs"
                Dim hrefOnBase As String
                hrefOnBase = fn_EjecutaStr("mis_WSOnBasePath " & CodAbona_)
                ' hrefOnBase = ws.ObtieneParametro(Cons.RutaWebServOnBase)
                hrefOnBase = Replace(hrefOnBase, "@Folio", FolioOnBase_)
                Funciones.EjecutaFuncion("window.open('" & hrefOnBase & "','_blank');")

            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "grdOrdenPago_RowCommand: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Firmar_Click(sender As Object, e As EventArgs) Handles btn_Firmar.Click
        Try

            If fn_Autorizaciones(False) = False Then
                Exit Sub
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Firmar_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Excel_Click(sender As Object, e As EventArgs) Handles btn_Excel.Click

        ExportToExcel()
    End Sub

  Private Sub ExportToExcel()

        'Create a dummy GridView
        Dim GridView1 As New GridView()
        Dim dtReporte As New DataTable

        dtReporte = dtOrdenPago.Copy

        GridView1.AllowPaging = False

        EliminaColumnasExcel(dtReporte)

        GridView1.DataSource = dtReporte
        GridView1.DataBind()

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition",
          "attachment;filename=OPsTradicional.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"

        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)

        For i As Integer = 0 To GridView1.Rows.Count - 1
            'Apply text style to each Row
            ' GridView1.Rows(i).Attributes.Add("class", "textmode")
            'GridView1.Rows(i).Cells(6).Text = GridView1.Rows(i).Cells(6).Text

        Next
        GridView1.RenderControl(hw)

        'style to format numbers to string

        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub

    Private Sub EliminaColumnasExcel(dtReporte As DataTable)

        'elimina columnas
        dtReporte.Columns.Remove("SolicitudPago")
        dtReporte.Columns.Remove("Folio_Onbase_est_cuenta")
        dtReporte.Columns.Remove("NumeroRecibo")
        dtReporte.Columns.Remove("NombreSucursal")
        dtReporte.Columns.Remove("NombreSucursalPago")
        dtReporte.Columns.Remove("CodigoAbona")
        dtReporte.Columns.Remove("NombreModifica")
        dtReporte.Columns.Remove("NombreUsuario")
        dtReporte.Columns.Remove("Observaciones")
        dtReporte.Columns.Remove("NombreAbona")
        dtReporte.Columns.Remove("Direccion")
        dtReporte.Columns.Remove("Calle")
        dtReporte.Columns.Remove("NumeroExterior")
        dtReporte.Columns.Remove("Colonia")
        dtReporte.Columns.Remove("CodigoPostal")
        dtReporte.Columns.Remove("Municipio")
        dtReporte.Columns.Remove("Ciudad")
        dtReporte.Columns.Remove("Departamento")
        dtReporte.Columns.Remove("Sector")
        dtReporte.Columns.Remove("CodigoBanco")
        dtReporte.Columns.Remove("NombreBanco")
        dtReporte.Columns.Remove("Swift")
        dtReporte.Columns.Remove("Aba")
        dtReporte.Columns.Remove("Transferencia")
        dtReporte.Columns.Remove("NumeroCuenta")
        dtReporte.Columns.Remove("Moneda")
        dtReporte.Columns.Remove("txt_moneda")
        dtReporte.Columns.Remove("CodigoAnulacion")
        dtReporte.Columns.Remove("ClaveProveedor")
        dtReporte.Columns.Remove("SubRamoContable")
        dtReporte.Columns.Remove("TipoTransferencia")
        dtReporte.Columns.Remove("NumeroPoliza")
        dtReporte.Columns.Remove("Contratante")
        dtReporte.Columns.Remove("ClasePago")
        dtReporte.Columns.Remove("Solicitante")
        dtReporte.Columns.Remove("Jefe")
        dtReporte.Columns.Remove("Tesoreria")
        dtReporte.Columns.Remove("Subdirector")
        dtReporte.Columns.Remove("Director")
        dtReporte.Columns.Remove("DirectorEjecutivo")
        dtReporte.Columns.Remove("DirectorGeneral")
        dtReporte.Columns.Remove("Subgerente")
        dtReporte.Columns.Remove("Gerente")
        dtReporte.Columns.Remove("NombreSolicitante")
        dtReporte.Columns.Remove("NombreJefe")
        dtReporte.Columns.Remove("NombreTesoreria")
        dtReporte.Columns.Remove("NombreSubdirector")
        dtReporte.Columns.Remove("NombreDirector")
        dtReporte.Columns.Remove("NombreDirectorEjecutivo")
        dtReporte.Columns.Remove("NombreDirectorGeneral")
        dtReporte.Columns.Remove("NombreSubgerente")
        dtReporte.Columns.Remove("NombreGerente")
        dtReporte.Columns.Remove("FirmadoSolicitante")
        dtReporte.Columns.Remove("FirmadoJefe")
        dtReporte.Columns.Remove("FirmadoSubdirector")
        dtReporte.Columns.Remove("FirmadoTesoreria")
        dtReporte.Columns.Remove("FirmadoDirector")
        dtReporte.Columns.Remove("FirmadoDirectorEjecutivo")
        dtReporte.Columns.Remove("FirmadoDirectorGeneral")
        dtReporte.Columns.Remove("FirmadoSubgerente")
        dtReporte.Columns.Remove("FirmadoGerente")
        dtReporte.Columns.Remove("FechaFirmaSolicitante")
        dtReporte.Columns.Remove("FechaFirmaJefe")
        dtReporte.Columns.Remove("FechaFirmaTesoreria")
        dtReporte.Columns.Remove("FechaFirmaSubdirector")
        dtReporte.Columns.Remove("FechaFirmaDirector")
        dtReporte.Columns.Remove("FechaFirmaDirectorEjecutivo")
        dtReporte.Columns.Remove("FechaFirmaDirectorGeneral")
        dtReporte.Columns.Remove("FechaFirmaSubgerente")
        dtReporte.Columns.Remove("FechaFirmaGerente")
        dtReporte.Columns.Remove("NivelAutorizacion")
        dtReporte.Columns.Remove("motivo_rechazo")
        dtReporte.Columns.Remove("usu_solrechazo")
        dtReporte.Columns.Remove("RoleUsuario")
        dtReporte.Columns.Remove("NumeroInterior")

        dtReporte.Columns(1).ColumnName = "OrdenPago"
        dtReporte.Columns(4).ColumnName = "Beneficiario"
        dtReporte.Columns(6).ColumnName = "Monto_Pago"
        dtReporte.Columns(7).ColumnName = "Tipo_Pago"
        dtReporte.Columns.Add("Estatus")

        For Each row In dtReporte.Rows
            Dim value As Object
            Dim value2 As Object

            value = row.Item("Preautorizada")
            value2 = row.Item("Rechazada")

            If Not value Is DBNull.Value Then
                If CStr(value) = 1 Then  'si esta autorizada
                    row.Item("Estatus") = "Autorizada"
                ElseIf CStr(value2) = 1 Then 'si esta rechazada
                    row.Item("Estatus") = "Rechazada"
                Else
                    row.Item("Estatus") = "Abierta"
                End If
            End If

        Next

        dtReporte.Columns.Remove("Preautorizada")
        dtReporte.Columns.Remove("Rechazada")



    End Sub

    Private Sub grdOrdenPago_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grdOrdenPago.RowDataBound
        If (e.Row.RowType = DataControlRowType.DataRow) Then
            If Not (e.Row.DataItem Is Nothing) Then
                Dim ddl_ConceptoRech_ As DropDownList = (TryCast(e.Row.FindControl("cmbConcepto"), DropDownList))
                Dim dtConceptos As New DataTable

                Funciones.fn_Consulta("mis_ConceptosAnulacion", dtConceptos)
                ddl_ConceptoRech_.DataValueField = "Clave"
                ddl_ConceptoRech_.DataTextField = "Descripcion"
                ddl_ConceptoRech_.DataSource = dtConceptos
                ddl_ConceptoRech_.DataBind()

                Dim opcion As ListItem
                opcion = New ListItem("--Seleccione--", "0")
                ddl_ConceptoRech_.Items.Add(opcion)

                Dim ultimoPts = ddl_ConceptoRech_.Items.Count
                ddl_ConceptoRech_.SelectedIndex = ultimoPts - 1
            End If
        End If

    End Sub



    'Protected Sub cmbConcepto_TextChanged(sender As Object, e As EventArgs)

    '    Dim drop As DropDownList = TryCast(grdOrdenPago.FindControl("cmbConcepto"), DropDownList)

    '    MsgBox(drop.SelectedValue)

    '    'Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, DropDownList).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)

    '    'Dim ddlRechazo = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txt_Motivo"), DropDownList)
    '    'Dim txtOtros = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txtOtros"), TextBox)

    '    'If ddlRechazo.SelectedValue = 11 Then
    '    '    'Mensaje.MuestraMensaje(Master.Titulo, "hola mundo", TipoMsg.Falla)
    '    '    txtOtros.Visible = True
    '    'Else
    '    '    txtOtros.Visible = False
    '    '    txtOtros.Text = ""
    '    'End If

    'End Sub
    'Protected Sub cmbConcepto_SelectedIndexChanged(sender As Object, e As EventArgs)

    '    Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, DropDownList).Parent.Parent, DataControlFieldCell).Parent, GridViewRow)
    '    Dim ddlRechazo = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("cmbConcepto"), DropDownList)
    '    Dim txtOtros = DirectCast(grdOrdenPago.Rows(gr.RowIndex).FindControl("txtMotivoOtro"), TextBox)

    '    If ddlRechazo.SelectedValue = 11 Then
    '        'Mensaje.MuestraMensaje(Master.Titulo, "hola mundo", TipoMsg.Falla)
    '        txtOtros.Visible = True
    '    Else
    '        txtOtros.Visible = False
    '        txtOtros.Text = ""
    '    End If

    'End Sub

End Class




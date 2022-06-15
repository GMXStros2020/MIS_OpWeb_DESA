Imports System.Data.SqlClient
Imports System.Data
Imports Mensaje
Imports Funciones

Partial Class Siniestros_CancelacionOps
    Inherits System.Web.UI.Page
    Public Property dtOrdenPago() As DataTable
        Get
            Return Session("dtOrdenPago")
        End Get
        Set(ByVal value As DataTable)
            Session("dtOrdenPago") = value
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

    Private Enum Operacion
        Ninguna
        Consulta
    End Enum

    Protected Sub chk_PorRevisar_CheckedChanged(sender As Object, e As EventArgs)
        VerificaRadios(Cons.TipoFiltro.PorRevisar)
    End Sub
    Protected Sub chk_Pendiente_CheckedChanged(sender As Object, e As EventArgs)
        VerificaRadios(Cons.TipoFiltro.Pendientes)
    End Sub
    Protected Sub chk_Rechazadas_CheckedChanged(sender As Object, e As EventArgs)
        VerificaRadios(Cons.TipoFiltro.Rechazadas)
    End Sub
    Private Sub VerificaRadios(tipo As Integer)

        Select Case tipo
            Case 0
                'If chk_Todas.Checked Then
                '    chk_Rechazadas.Checked = False
                '    chk_PorRevisar.Checked = False
                '    chk_Pendiente.Checked = False
                '    chk_Autorizada.Checked = False
                '    chk_FinalAut.Checked = False
                '    chk_Revisadas.Checked = False
                '    ddlRolFilter.Visible = False
                '    div_Fechas.Visible = False
                'End If
            Case 1
                If chk_PorRevisar.Checked Then
                    chk_Rechazadas.Checked = False
                    chk_Pendiente.Checked = False
                    'chk_Todas.Checked = False
                End If
            Case 2
                If chk_Pendiente.Checked Then
                    'chk_Todas.Checked = False
                    chk_PorRevisar.Checked = False
                    chk_Rechazadas.Checked = False

                End If
            Case 4
                If chk_Rechazadas.Checked Then
                    'chk_Todas.Checked = False
                    chk_PorRevisar.Checked = False
                    chk_Pendiente.Checked = False
                End If

        End Select

    End Sub

    Public Function BuscarControlPorClase(ByVal rootControl As Control, ByVal controlCss As String) As Control

        Dim control As Control = Nothing

        Try

            For Each controlToSearch As Control In rootControl.Controls

                Select Case controlToSearch.GetType().Name

                    Case "DropDownList"

                        Dim ddl As DropDownList = controlToSearch

                        If ddl.CssClass = controlCss Then
                            control = ddl
                            Exit For
                        End If

                    Case Else

                        Dim controlToReturn As Control = BuscarControlPorClase(controlToSearch, controlCss)

                        If controlToReturn IsNot Nothing Then
                            Return controlToReturn
                        End If

                End Select

            Next

        Catch ex As Exception
            control = Nothing
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("BuscarControlPorClase error: {0}", ex.Message), TipoMsg.Falla)
        End Try
        Return control

    End Function

    Private Sub CargaGridDDL()
        Dim cmbConceptoCancelacion As DropDownList

        Dim oDatos As DataSet
        oDatos = New DataSet

        cmbConceptoCancelacion = New DropDownList

        cmbConceptoCancelacion = BuscarControlPorClase(grdOrdenPago, "estandar-control concepto_cancel")

        oDatos = Funciones.ObtenerDatos("mis_CatCptosAnulacion")

        If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

            If cmbConceptoCancelacion.Items.Count > 0 Then
                cmbConceptoCancelacion.Items.Clear()
            End If
            cmbConceptoCancelacion.DataSource = oDatos
            cmbConceptoCancelacion.DataTextField = "desc_concepto_anulacion"
            cmbConceptoCancelacion.DataValueField = "cod_concepto_anulacion"
            cmbConceptoCancelacion.DataBind()
        End If
    End Sub

    Private Function ValidaRadios() As Boolean
        ValidaRadios = True
        'If chk_Todas.Checked = False Then
        If chk_PorRevisar.Checked = False Then
            If chk_Pendiente.Checked = False Then
                If chk_Rechazadas.Checked = False Then
                    ValidaRadios = False
                End If
            End If
        End If
        'End If

    End Function

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
            sFiltroUsuario = IIf(cmbElaborado.SelectedValue <> "-1", cmbElaborado.SelectedValue, String.Empty)

            If sFiltroOP <> "" Then
                Dim Rechazada As Integer = fn_Ejecuta("mis_ValidaStsOp " & sFiltroOP)
                If Rechazada = 1 Then
                    Mensaje.MuestraMensaje("Validación", "la Orden de Pago: " & sFiltroOP & " ya se encuentra rechazada", TipoMsg.Advertencia)
                    ConsultaOrdenesPagoSiniestros = Nothing
                    Exit Function
                End If
            End If

            'sFiltroUsuario = IIf(Not String.IsNullOrWhiteSpace(sFiltroUsuario), String.Format("AND t.cod_usuario IN ('{0}')", sFiltroUsuario), String.Empty)


            If IsDate(txtFechaGeneracionDesde.Text.Trim) And IsDate(txtFechaGeneracionHasta.Text.Trim) Then
                If CDate(txtFechaGeneracionDesde.Text.Trim) <= CDate(txtFechaGeneracionHasta.Text.Trim) Then
                    sFiltroFechaGeneracion = String.Format(" AND CONVERT(VARCHAR(10),fec_generacion,112) >= ''{0}'' AND CONVERT(VARCHAR(10),fec_generacion,112) <= ''{1}'' ", CDate(txtFechaGeneracionDesde.Text).ToString("yyyyMMdd"), CDate(txtFechaGeneracionHasta.Text).ToString("yyyyMMdd"))
                End If
            End If

            If chk_PorRevisar.Checked Then
                iStatusFirma = Cons.TipoFiltro.PorRevisar
            ElseIf chk_Pendiente.Checked Then
                iStatusFirma = Cons.TipoFiltro.Pendientes
            ElseIf chk_Rechazadas.Checked Then
                iStatusFirma = Cons.TipoFiltro.Rechazadas
            End If

            Dim valorMoneda As String = ""
            'If cmbMoneda.SelectedItem.Text <> ". . ." Then valorMoneda = cmbMoneda.SelectedItem.Text

            If txtSiniestro.Text <> "" Then sFiltroStro = txtSiniestro.Text
            'If txtAsegurado.Text <> "" Then sFiltroBenef = txtAsegurado.Text


            Dim ValorRol As Integer = 0
            'ValorRol = ddlRolFilter.SelectedValue

            '"CLOPEZ", 'Master.cod_usuario,
            '0, 'iStatusFirma,
            'Cambiar SP por original (usp_ObtenerOrdenPago_stro)
            fn_Consulta(String.Format("usp_ObtenerOrdenPago_stro_T '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}','{13}','{14}'",
                                              Cons.StrosTradicional,
                                              sFiltroOP,
                                              sFiltroMonto,
                                              sFiltroFechaGeneracion,
                                              sFiltroFechaPago,
                                              sFiltroUsuario,
                                              "CLOPEZ",
                                              0,
                                              ValorRol,
                                               valorMoneda,
                                                sFiltroStro,
                                                sFiltroBenef,
                                                sFiltroFecDe,
                                                sFiltroFecHasta,
                                                1), dtOrdenPago)

            Return dtOrdenPago

            'End If

        Catch ex As Exception
            ConsultaOrdenesPagoSiniestros = Nothing
            Mensaje.MuestraMensaje(Master.Titulo, String.Format("ConsultaOrdenesPagoSiniestros Error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Function
    Private Sub BarraEstados(valor As Boolean)
        'chk_Todas.Enabled = valor
        chk_PorRevisar.Enabled = valor

        chk_Pendiente.Enabled = valor

        chk_Rechazadas.Enabled = valor
    End Sub
    Private Sub EdoControl(ByVal intOperacion As Integer)
        Select Case intOperacion
            Case Operacion.Consulta
                txt_NroOP.Enabled = False
                txtFechaGeneracionDesde.Enabled = False
                txtFechaGeneracionHasta.Enabled = False

                BarraEstados(False)
                txtSiniestro.Enabled = False

                chk_Pendiente.Enabled = False

                btn_BuscaOP.Visible = False

                'btn_Imprimir.Visible = True


                btn_Firmar.Visible = True


                hid_Ventanas.Value = "1|0|1|1|1|1|1|1|"

            Case Operacion.Ninguna
                Funciones.LlenaGrid(grdOrdenPago, Nothing)
                txt_NroOP.Enabled = True

                txtFechaGeneracionDesde.Enabled = True
                txtFechaGeneracionHasta.Enabled = True

                LimpiaCtrls()
                BarraEstados(True)
                txtSiniestro.Enabled = True
                chk_Pendiente.Enabled = True

                chk_Pendiente.Checked = False

                'btn_Todas.Visible = False
                'btn_Ninguna.Visible = False

                'btn_Imprimir.Visible = False
                'btn_SelTodos.Visible = False
                'btn_Firmar.Visible = False
                btn_BuscaOP.Visible = True

                hid_Ventanas.Value = "0|1|1|1|1|1|1|1|"

                hid_rechazo.Value = 0

        End Select
    End Sub
    Private Sub LimpiaCtrls()
        txt_NroOP.Text = ""

        txtFechaGeneracionDesde.Text = ""
        txtFechaGeneracionHasta.Text = ""

        chk_PorRevisar.Checked = False

        'chk_Todas.Checked = False
        chk_Pendiente.Checked = False
        chk_Rechazadas.Checked = False

        txtSiniestro.Text = ""




    End Sub
    Private Sub btn_BuscaOP_Click(sender As Object, e As EventArgs) Handles btn_BuscaOP.Click
        Try
            'If cmbModuloOP.SelectedValue > 0 Then
            'If ValidaRadios() Then
            'Funciones.LlenaGrid(grdOrdenPago, ConsultaOrdenesPagoSiniestros(cmbModuloOP.SelectedValue))
            Funciones.LlenaGrid(grdOrdenPago, ConsultaOrdenesPagoSiniestros(Cons.StrosTradicional))
            'Carga grid con conceptos
            ' CargaGridDDL()

            If grdOrdenPago.Rows.Count > 0 Then
                'grdOrdenPago.PageIndex = 0

                'For Each renglon In grdOrdenPago.Rows
                '    CargaGridDDL()

                'Next

                EdoControl(Operacion.Consulta)
                'DesHabilitaChecksFirma()
                Funciones.EjecutaFuncion("fn_EstadoFilas('grdOrdenPago',true);")
            Else
                Mensaje.MuestraMensaje(Master.Titulo, "La Consulta no devolvió resultados", TipoMsg.Advertencia)
                End If
            'Else
            '    MuestraMensaje("Validación", "Debe elegir un filtro de Estatus", TipoMsg.Advertencia)
            'End If
            'Else
            '    MuestraMensaje("Validación", "Debe elegir el tipo de módulo", TipoMsg.Advertencia)
            'End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_BuscaOP_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub Siniestros_CancelacionOps_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then
                Master.Titulo = "Cancelación de Ordenes de Pago"
                Master.cod_modulo = Cons.ModuloStrosTec 'Cons.ModuloRea
                Master.cod_submodulo = Cons.SubModCancelacion

                Master.InformacionGeneral()
                Master.EvaluaPermisosModulo()

                EdoControl(Operacion.Ninguna)
                dtOrdenPago = Nothing


                'Dim Params As String = Request.QueryString("NumOrds")
                'If Params <> vbNullString Then
                '    txt_NroOP.Text = Split(Params, "|")(0)
                '    'cmbModuloOP.SelectedValue = Split(Params, "|")(1)
                '    Master.cod_usuario = Split(Params, "|")(2)
                '    chk_Todas.Checked = True
                'End If

                'If Len(txt_NroOP.Text) > 0 Then
                '    Master.cod_usuario = Split(Context.User.Identity.Name, "|")(0)
                '    btn_BuscaOP_Click(Me, Nothing)
                '    Funciones.EjecutaFuncion("fn_EstadoFilas('grdOrdenPago', false);", "Filas")
                'End If
                'LlenaCatDDL(cmbElaborado, "UsuStro",,,,, -1)
                Dim DtUsuStro As New DataTable
                fn_Consulta("mis_UsuStroAdmin", DtUsuStro)
                If Not DtUsuStro Is Nothing Then
                    LlenaDDL(cmbElaborado, DtUsuStro,,, -1)

                End If


            End If
            EstadoDetalleOrden()
            'Master.cod_usuario = "CLOPEZ"
            ' ValidaUsrFiltros()



        Catch ex As Exception
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "OrdenPago_Cancelacion_Load: " & ex.Message)
        End Try
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

    Private Sub grdOrdenPago_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grdOrdenPago.RowCommand
        If e.CommandName = "Visualizar" Then
            Try
                Dim strOrdenPago As String = "-1"

                Dim server As String = String.Empty

                Dim RowIndex As Integer = Convert.ToInt32(e.CommandArgument.ToString())

                'Dim nro_opsel As Label = TryCast(grdOrdenPago.Rows(RowIndex).FindControl("nro_op"), Label)
                'ActualizaDataOP()
                strOrdenPago = dtOrdenPago.Rows(RowIndex).ItemArray(2).ToString() 'Obtiene numero de OP

                Dim ws As New ws_Generales.GeneralesClient

                server = ws.ObtieneParametro(Cons.TargetReport)
                server = Replace(Replace(server, "@Reporte", "OrdenPago"), "@Formato", "PDF") & "&nro_op=@nro_op"
                server = Replace(server, Cons.ReposSource, Cons.ReposReport)
                server = Replace(server, "OrdenPago", "OrdenPago_stro")

                'For Each row In grdOrdenPago.Rows

                '    If TryCast(row.FindControl("chk_Print"), CheckBox).Checked Then
                '        strOrdenPago = String.Format("{0}, {1}", strOrdenPago, DirectCast(row.FindControl("lblOrdenPago"), Label).Text.Trim)
                '    End If

                'Next

                If strOrdenPago <> "-1" Then

                    strOrdenPago = Replace(strOrdenPago, "-1,", "")

                    Funciones.EjecutaFuncion(String.Format("fn_Imprime_OP('{0}','{1}');",
                                                                   server,
                                                                   strOrdenPago))

                End If

            Catch ex As Exception
                Mensaje.MuestraMensaje("Eliminar Estatus", "Ocurrio un Error al eliminar el registro", TipoMsg.Falla)
            End Try
        End If
    End Sub
    Protected Sub cmbConcepto_SelectedIndexChanged(sender As Object, e As EventArgs)

        Dim ddl As DropDownList = DirectCast(sender, DropDownList)
        Dim row As GridViewRow = DirectCast(ddl.Parent.Parent, GridViewRow)
        Dim idx As Integer = row.RowIndex


        Dim ddlRechazo = DirectCast(grdOrdenPago.Rows(idx).FindControl("cmbConcepto"), DropDownList)
        Dim txtOtros = DirectCast(grdOrdenPago.Rows(idx).FindControl("txtOtros"), TextBox)

        If ddlRechazo.SelectedValue = 11 Then
            txtOtros.Visible = True
        Else
            txtOtros.Visible = False
            txtOtros.Text = ""
        End If
    End Sub


    Private Function fn_Cancelaciones(ByVal sn_proceso As Boolean) As Boolean
        Dim strOP As String = ""
        Dim codRol As String = ""
        Dim OPCompletada As Boolean = False

        Dim ws As New ws_Generales.GeneralesClient

        fn_Cancelaciones = False

        'Nivel 1 Firma analista y supervisor
        'Nivel 2 Firma analista y subgerente
        'Nivel 3 Firma analista y subdirector
        'Nivel 4 Firma analista, subdirector y director
        'Nivel 5 Firma analista, subdirector, director y director general

        Dim UsuarioFirma As String = vbNullString
        Dim contador As Integer = 0
        Dim ResultTran As Integer = 0

        dtCancela = New DataTable
        dtCancela.Columns.Add("noOP")
        dtCancela.Columns.Add("Justificacion")

        Dim UsrElaboro As String = ""
        Dim UsrSolicito As String = ""

        For Each row In grdOrdenPago.Rows


            'If contadorIni = dtAutorizaciones.Rows.IndexOf(row) And contador <= (RowsMostrados - 1) Then
            Dim chk_Rechazo As Boolean = DirectCast(grdOrdenPago.Rows(contador).FindControl("chkCancel"), CheckBox).Checked

            If chk_Rechazo = True Then
                Dim ddlMotivo = DirectCast(grdOrdenPago.Rows(contador).FindControl("cmbConcepto"), DropDownList)

                Dim txtJustif As String = DirectCast(grdOrdenPago.Rows(contador).FindControl("cmbConcepto"), DropDownList).SelectedItem.Text
                Dim txtOtros As String = DirectCast(grdOrdenPago.Rows(contador).FindControl("txtOtros"), TextBox).Text

                If ddlMotivo.SelectedValue = 11 Then
                    txtJustif = txtOtros
                End If

                strOP = DirectCast(grdOrdenPago.Rows(contador).FindControl("nro_op_"), Label).Text

                Dim Rechazada As Integer = fn_Ejecuta("mis_ValidaStsOp " & strOP)
                If Rechazada = 1 Then
                    Mensaje.MuestraMensaje("Validación", "la Orden de Pago: " & strOP & " ya se encuentra rechazada, por favor deseleccionarla", TipoMsg.Advertencia)
                    Exit Function
                End If

                Dim codMotivoRechazo As Integer
                Dim strMotivoRechazo As String
                Dim intFolioOnBase As Integer
                codMotivoRechazo = DirectCast(grdOrdenPago.Rows(contador).FindControl("cmbConcepto"), DropDownList).SelectedItem.Value
                strMotivoRechazo = DirectCast(grdOrdenPago.Rows(contador).FindControl("cmbConcepto"), DropDownList).SelectedItem.Text
                intFolioOnBase = DirectCast(grdOrdenPago.Rows(contador).FindControl("folioonbase"), Label).Text

                If strMotivoRechazo = "--Seleccione--" Then
                    Mensaje.MuestraMensaje("Validación Motivo de Rechazo", "No se ha seleccionado ningun motivo de rechazo de para la OP: " & strOP, TipoMsg.Advertencia)
                    Exit Function
                End If

                If sn_proceso = True Then

                    'MMQ se comentan la aplicacion y el correo que ahora seran lanzados todos dentro del mis_CancelaOpStros
                    ' fn_Ejecuta("usp_AplicaFirmasOP_stro " & strOP & ",0,'" & codRol & "',0,'Usuario: " & Master.usuario & " /Motivo: " & strMotivoRechazo & "'")

                        fn_Ejecuta("mis_CancelaOPStros " & strOP & ",'" & Master.cod_usuario & "'," & codMotivoRechazo & "," & intFolioOnBase & ",'" & codRol & "','" & strMotivoRechazo & "'")

                    '  fn_Ejecuta("mis_MailOpRechazo '" & strOP & "','CLOPEZ','" & Master.usuario & "'")

                    UsrElaboro = ""
                    UsrSolicito = ""

                    UsrElaboro = fn_EjecutaStr("spS_CancelRemitentes " & CInt(strOP) & "," & 0) 'Usuario elaboro OP
                    UsrSolicito = fn_EjecutaStr("spS_CancelRemitentes " & CInt(strOP) & "," & 1) 'Usuario solicito rechzo OP

                    If Len(UsrSolicito) > 0 Then
                        fn_Ejecuta("mis_MailOpRechazo '" & strOP & "','" & UsrSolicito & "','" & Master.usuario & "'")
                    End If

                    If Len(UsrElaboro) > 0 Then
                        fn_Ejecuta("mis_MailOpRechazo '" & strOP & "','" & UsrElaboro & "','" & Master.usuario & "'")
                    End If

                Else
                    dtCancela.Rows.Add(strOP, txtJustif)
                End If



            End If
            contador = contador + 1
        Next

        gvd_Canceladas.DataSource = dtCancela
        gvd_Canceladas.DataBind()

        'If dtCancela Is Nothing Then
        If dtCancela.Rows.Count = 0 And sn_proceso = False Then

            Mensaje.MuestraMensaje(Master.Titulo, "No se ha seleccionado ninguna Orden de Pago para cancelar", TipoMsg.Advertencia)
            fn_Cancelaciones = False
            Exit Function

        Else

            gvd_Canceladas.DataSource = dtCancela
            gvd_Canceladas.DataBind()

        End If
        ' fn_Cancelaciones = True

        If sn_proceso = False Then
            Funciones.AbrirModal("#Resumen")
            fn_Cancelaciones = True
        Else
            fn_Cancelaciones = False
        End If


    End Function

    Private Sub btn_Firmar_Click(sender As Object, e As EventArgs) Handles btn_Firmar.Click
        Try

            If fn_Cancelaciones(False) = False Then
                Exit Sub
            End If



        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Firmar_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub lnkAceptarProc_Click(sender As Object, e As EventArgs) Handles lnkAceptarProc.Click

        Try


            If fn_Cancelaciones(True) = False Then
                Mensaje.MuestraMensaje("Confirmación de cancelación", "Se realizaron las cancelaciones correctamente", TipoMsg.Confirma)
                btn_Limpiar_Click(Nothing, Nothing)
                Funciones.CerrarModal("#Resumen")
                Exit Sub
            End If



        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Firmar_Click: " & ex.Message)
        End Try

    End Sub

    Private Sub btn_Limpiar_Click(sender As Object, e As EventArgs) Handles btn_Limpiar.Click
        Try

            EdoControl(Operacion.Ninguna)
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_Limpiar_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_BuscaOP_Command(sender As Object, e As CommandEventArgs) Handles btn_BuscaOP.Command

    End Sub
End Class

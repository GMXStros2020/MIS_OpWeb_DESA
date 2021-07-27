Imports System.Data
Imports System.Data.SqlClient
Imports Mensaje
Imports Funciones
Imports System.IO
Partial Class Siniestros_CartasChequeAutorizacion
    Inherits System.Web.UI.Page
    Dim oTabla As DataTable
    Dim bflag As Boolean

    Private Enum TipoFiltro
        Todas = 0
        Pendientes = 1
        Autorizadas = 2
        Rechazadas = 3
    End Enum

    Public Property dtToken() As DataTable
        Get
            Return Session("dtToken")
        End Get
        Set(ByVal value As DataTable)
            Session("dtToken") = value
        End Set
    End Property

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        'Dim oDatos As DataSet
        'Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim FolioCarta As String
        'Dim Fondos As String

        FolioCarta = Request.QueryString("folio")

        If Not IsPostBack Then
            Master.InformacionGeneral()
            If FolioCarta <> Nothing Then

                'oParametros.Add("folio_gmx", FolioCarta)
                'Dim usuario As String
                'usuario = Master.cod_usuario

                ''Dim DetalleUsuario() As String
                ''DetalleUsuario = Split(Context.User.Identity.Name, "|")

                'oParametros.Add("cod_usuario", Master.cod_usuario)
                'oDatos = Funciones.ObtenerDatos("usp_buscar_datos_carta_ch_autoriza", oParametros)

                'oTabla = oDatos.Tables(0)

                'grd.DataSource = oTabla

                'grd.DataBind()
                txt_folio_carta.Text = FolioCarta
                Buscar()
            End If
        End If





    End Sub

    Protected Sub btn_BuscarOP_Click(sender As Object, e As EventArgs) Handles btn_BuscarOP.Click
        Buscar()
    End Sub
    Private Sub Buscar()
        btnAutorizar.Visible = False
        'btn_Todas.Visible = False      'ajustes cartas 06112020
        'btn_Ninguna.Visible = False    'ajustes cartas 06112020


        If Not ValidaFiltros() Then Exit Sub
        oTabla = BuscarOP()
        'Session("dtOP") = oTabla

        If Not IsNothing(oTabla) Then
            If oTabla.Rows.Count > 0 Then
                btnAutorizar.Visible = True
                'btn_Todas.Visible = True       'ajustes cartas 06112020
                'btn_Ninguna.Visible = True     'ajustes cartas 06112020

            End If
        End If
        grd.DataSource = oTabla
        grd.DataBind()
    End Sub

    Private Function BuscarOP() As DataTable
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Try
            oParametros.Add("nro_op_desde", ValidarParametros(Trim(txt_nro_op.Text)))
            oParametros.Add("nro_op_hasta", ValidarParametros(Trim(txt_nro_op_hasta.Text)))
            oParametros.Add("nro_ch_desde", ValidarParametros(Trim(txt_nro_ch_desde.Text)))
            oParametros.Add("nro_ch_hasta", ValidarParametros(Trim(txt_nro_ch_hasta.Text)))
            oParametros.Add("nro_stro", ValidarParametros(Trim(txt_nro_stro.Text)))
            oParametros.Add("txt_cheque_a_nom", ValidarParametros(Trim(txt_cheque_a_nom.Text)))
            oParametros.Add("folio_gmx", ValidarParametros(Trim(txt_folio_carta.Text)))

            oParametros.Add("pendientes", IIf(chk_Pendientes.Checked = True, -1, 0))
            oParametros.Add("autorizadas", IIf(chk_Autorizadas.Checked = True, -1, 0))
            oParametros.Add("rechazadas", IIf(chk_Rechazadas.Checked = True, -1, 0))
            oParametros.Add("todas", IIf(chk_Todas.Checked = True, -1, 0))
            oParametros.Add("cod_usuario", Master.cod_usuario)



            oDatos = Funciones.ObtenerDatos("usp_buscar_datos_carta_ch_autoriza", oParametros)
            oTabla = oDatos.Tables(0)



            Return oTabla

        Catch ex As Exception
            MuestraMensaje("Exception", "BuscarOP: " & ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try
    End Function
    Private Function ValidaFiltros() As Boolean
        Dim nroOpDesde As Double
        Dim nroOpHasta As Double
        Dim nroChequeDesde As Double
        Dim nroChequeHasta As Double

        ValidaFiltros = True




        If Trim(txt_nro_op.Text) = "" Then
            If Trim(txt_nro_op_hasta.Text) = "" Then
                If Trim(txt_nro_ch_desde.Text) = "" Then
                    If Trim(txt_nro_ch_hasta.Text) = "" Then
                        If Trim(txt_nro_stro.Text) = "" Then
                            If Trim(txt_folio_carta.Text) = "" Then
                                If Trim(txt_cheque_a_nom.Text) = "" Then
                                    If Not chk_Todas.Checked Then
                                        If Not chk_Pendientes.Checked Then
                                            If Not chk_Autorizadas.Checked Then
                                                If Not chk_Rechazadas.Checked Then
                                                    MuestraMensaje("Validación", "Debe elegir al menos un filtro para la consulta", TipoMsg.Advertencia)
                                                    ValidaFiltros = False
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Else
                        MuestraMensaje("Validación", "Debe seleccionar Núm Cheque desde", TipoMsg.Advertencia)
                        ValidaFiltros = False
                    End If
                Else
                    If Not Trim(txt_nro_ch_hasta.Text) = "" Then
                        nroChequeDesde = Convert.ToDouble(Trim(txt_nro_ch_desde.Text))
                        nroChequeHasta = Convert.ToDouble(Trim(txt_nro_ch_hasta.Text))

                        If nroChequeDesde > nroChequeHasta Then
                            MuestraMensaje("Validación", "Número Cheque desde debe ser mayor que número de cheque hasta", TipoMsg.Advertencia)
                            ValidaFiltros = False
                        End If
                    End If
                End If
            Else
                MuestraMensaje("Validación", "Debe seleccionar Núm OP desde", TipoMsg.Advertencia)
                ValidaFiltros = False
            End If
        Else
            If Not Trim(txt_nro_op_hasta.Text) = "" Then
                nroOpDesde = Convert.ToDouble(Trim(txt_nro_op.Text))
                nroOpHasta = Convert.ToDouble(Trim(txt_nro_op_hasta.Text))

                If nroOpDesde > nroOpHasta Then
                    MuestraMensaje("Validación", "Número OP desde debe ser mayor que Número de OP hasta", TipoMsg.Advertencia)
                    ValidaFiltros = False
                End If
            End If


            If Not Trim(txt_nro_ch_desde.Text) = "" Then
                If Not Trim(txt_nro_ch_hasta.Text) = "" Then
                    nroChequeDesde = Convert.ToDouble(Trim(txt_nro_ch_desde.Text))
                    nroChequeHasta = Convert.ToDouble(Trim(txt_nro_ch_hasta.Text))

                    If nroChequeDesde > nroChequeHasta Then
                        MuestraMensaje("Validación", "Número Cheque desde debe ser mayor que número de cheque hasta", TipoMsg.Advertencia)
                        ValidaFiltros = False
                    End If
                End If
            Else
                If Not Trim(txt_nro_ch_hasta.Text) = "" Then
                    MuestraMensaje("Validación", "Debe seleccionar Núm de Cheque desde", TipoMsg.Advertencia)
                    ValidaFiltros = False
                End If
            End If






        End If

    End Function

    Private Function ValidarParametros(txt_campo As String) As String
        Try
            If IsNothing(txt_campo) Then
                txt_campo = Nothing
                Return Nothing
                Exit Function
            End If

            If txt_campo.Length = 0 Then
                txt_campo = Nothing
                Return Nothing
                Exit Function
            End If

            If txt_campo.Length > 0 Then
                Return txt_campo
            End If

            Return Nothing
        Catch ex As Exception
            Return Nothing

        End Try
    End Function


    Private Function obtenerSeleccionados() As DataTable


        Try
            Dim dtAut As New DataTable
            Dim add As Boolean
            Dim indexTabla As Integer
            Dim reg As Integer
            add = True
            indexTabla = 0
            reg = 0

            If grd.Rows.Count = 0 Then
                MuestraMensaje("Validacion", "No existen registros en la tabla", TipoMsg.Advertencia)
            End If


            For Each cell As TableCell In grd.HeaderRow.Cells
                dtAut.Columns.Add(cell.Text)
                If cell.Text = "ACCION" Then
                    dtAut.Columns.Add("MOT_RECHAZO")
                End If
            Next


            For index = 0 To grd.Rows.Count - 1
                Dim eliminar As CheckBox
                eliminar = grd.Rows(index).FindControl("CheckBox1")
                If eliminar.Checked Then
                    dtAut.Rows.Add()
                End If
            Next

            For Each row As GridViewRow In grd.Rows
                For i As Integer = 0 To row.Cells.Count - 1
                    Dim selecc As CheckBox
                    selecc = row.Cells(i).FindControl("CheckBox1")
                    Dim accion As DropDownList
                    accion = row.Cells(i).FindControl("dropEstado")
                    Dim mot_rechazo As TextBox
                    mot_rechazo = row.Cells(i).FindControl("txt_mot_rech")

                    If selecc.Checked Then

                        Select Case i
                            Case 9
                                dtAut.Rows(indexTabla)(i) = accion.Text.ToString()
                            Case 10
                                dtAut.Rows(indexTabla)(i) = mot_rechazo.Text.ToString()
                            Case Else
                                dtAut.Rows(indexTabla)(i) = row.Cells(i).Text
                        End Select


                        add = True
                    Else
                        add = False
                    End If
                Next
                If add Then
                    indexTabla = indexTabla + 1
                End If
            Next

            Return dtAut
        Catch ex As Exception
            MuestraMensaje("Exception", "obtenerSeleccionados: " & ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try
    End Function

    Private Function autorizaCheques() As Boolean
        Dim oParametros As New Dictionary(Of String, Object)
        Dim dtA As DataTable
        Dim folios As String
        folios = ""
        Try
            dtA = Session("dtAutorizo")

            Dim dtR = New DataTable
            dtR = Session("dtRechazo")

            If dtA.Rows.Count = 0 And dtR.Rows.Count = 0 Then
                Return False
                Exit Function
            End If


            For Each row As DataRow In dtA.Rows
                folios = folios & IIf(folios = "", row("folioCarta").ToString, " ," & row("folioCarta").ToString)
                oParametros = New Dictionary(Of String, Object)
                oParametros.Add("folio_carta_gmx", ValidarParametros(row("folioCarta")))
                oParametros.Add("cod_ususario", Master.cod_usuario.ToString())
                oParametros.Add("status", 2)
                Funciones.ObtenerDatos("usp_datos_carta_ch_autoriza", oParametros)
            Next


            For Each row As DataRow In dtR.Rows
                folios = folios & IIf(folios = "", row("folioCarta").ToString, " ," & row("folioCarta").ToString)
                oParametros = New Dictionary(Of String, Object)
                oParametros.Add("folio_carta_gmx", ValidarParametros(row("folioCarta")))
                oParametros.Add("cod_ususario", Master.cod_usuario.ToString())
                oParametros.Add("status", 3)
                oParametros.Add("mot_rechazo", ValidarParametros(row("motRechazo")))
                Funciones.ObtenerDatos("usp_datos_carta_ch_autoriza", oParametros)
            Next

            Session("folios") = folios
            Return True
        Catch ex As Exception
            MuestraMensaje("Exception", "autorizaCheques: " & ex.Message, TipoMsg.Falla)
            Return False
        End Try
    End Function

    'ajustes cartas 06112020
    'Private Sub btn_Todas_Click(sender As Object, e As EventArgs) Handles btn_Todas.Click
    '    Try

    '        For Each row In grd.Rows
    '            Dim chkImp As CheckBox = TryCast(row.FindControl("CheckBox1"), CheckBox)
    '            If chkImp.Enabled = True Then
    '                chkImp.Checked = True
    '            End If
    '        Next
    '    Catch ex As Exception

    '        Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)

    '    End Try
    'End Sub

    'Private Sub btn_Ninguna_Click(sender As Object, e As EventArgs) Handles btn_Ninguna.Click
    '    Try
    '        For Each row In grd.Rows
    '            Dim chkImp As CheckBox = TryCast(row.FindControl("CheckBox1"), CheckBox)
    '            If chkImp.Enabled = True Then
    '                chkImp.Checked = False
    '            End If

    '        Next

    '    Catch ex As Exception
    '        Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)

    '    End Try

    'End Sub

    Protected Sub grd_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grd.RowCommand

        If bflag Then
            bflag.ToString()
        End If

        If e.CommandName = "RepCarta" Then
            Dim index As Integer
            Dim folio_carta As Integer

            e.CommandArgument.ToString()

            index = CInt(e.CommandArgument.ToString())
            folio_carta = CInt(grd.DataKeys(index).Value.ToString())

            generaReporte(folio_carta)

            bflag = True
            Exit Sub
        End If


        If e.CommandName = "rechazo" Then
            Dim indice As Integer
            Dim folio_carta As Integer

            e.CommandArgument.ToString()

            indice = CInt(e.CommandArgument.ToString())
            folio_carta = CInt(grd.DataKeys(indice).Value.ToString())



            bflag = True
            Exit Sub
        End If

    End Sub

    Private Sub generaReporte(nrofolio As Integer)
        Dim ws As New ws_Generales.GeneralesClient
        Dim server As String = ws.ObtieneParametro(Cons.TargetReport)

        Dim RptFilters As String
        RptFilters = "&folio=" & nrofolio.ToString()
        server = Replace(Replace(server, "@Reporte", "RepCartasCheque"), "@Formato", "PDF")
        server = Replace(server, Cons.ReposSource, Cons.ReposReport)

        server = server & RptFilters
        Funciones.EjecutaFuncion("window.open('" & server & "');")

    End Sub

    Private Sub btnAutorizar_Click(sender As Object, e As EventArgs) Handles btnAutorizar.Click
        Dim dtSelec As DataTable
        Dim msgError As String
        Dim foliosError As String
        dtSelec = obtenerSeleccionados()
        foliosError = ""


        If dtSelec.Rows.Count > 0 Then

            Dim dtAutorizo = New DataTable
            Dim dtRechazo = New DataTable
            dtAutorizo.Columns.Add("folioCarta")
            dtRechazo.Columns.Add("folioCarta")
            dtRechazo.Columns.Add("motRechazo")

            For Each row As DataRow In dtSelec.Rows
                If row("ACCION").ToString() = "3" Then
                    If row("MOT_RECHAZO").ToString() = "" Then
                        foliosError = foliosError & row("FOLIO CARTA").ToString() & ", "
                    End If

                End If
            Next

            If foliosError <> "" Then
                msgError = "Falta capturar motivo de rechazo para los folios " & foliosError
                MuestraMensaje("Motivo Rechazo", msgError, TipoMsg.Advertencia)
                Exit Sub
            End If


            For Each row As DataRow In dtSelec.Rows
                If row("ACCION").ToString() = "2" Then
                    dtAutorizo.Rows.Add(row("FOLIO CARTA").ToString())
                ElseIf row("ACCION").ToString() = "3" Then
                    dtRechazo.Rows.Add(row("FOLIO CARTA").ToString(), row("MOT_RECHAZO").ToString())
                End If
            Next

            If dtAutorizo.Rows.Count > 0 Then
                gvd_Autorizadas.DataSource = dtAutorizo
                gvd_Autorizadas.DataBind()
                Session("dtAutorizo") = dtAutorizo
            Else
                gvd_Autorizadas.DataSource = ""
                gvd_Autorizadas.DataBind()
                Dim dtt = New DataTable
                Session("dtAutorizo") = dtt
            End If

            If dtRechazo.Rows.Count > 0 Then
                gvd_Rechazadas.DataSource = dtRechazo
                gvd_Rechazadas.DataBind()
                Session("dtRechazo") = dtRechazo
            Else
                gvd_Rechazadas.DataSource = ""
                gvd_Rechazadas.DataBind()
                Dim dtt = New DataTable
                Session("dtRechazo") = dtt
            End If

            Funciones.AbrirModal("#Resumen")

        Else
            MuestraMensaje("Validación", "No existen registros seleccionados para autorizar o rechazar", TipoMsg.Advertencia)
        End If
    End Sub

    Private Function fn_Token() As Boolean
        Try
            Funciones.fn_Consulta("usp_Genera_Token_CartaCheque '" & Master.cod_usuario & "'", dtToken)
            If dtToken.Rows.Count > 0 Then hid_Token.Value = dtToken(0).ItemArray(0).ToString
            Return True
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, String.Format("Token Error: {0}", ex.Message), TipoMsg.Falla)
            Return False
        End Try

    End Function

    Private Sub lnkAceptarProc_Click(sender As Object, e As EventArgs) Handles lnkAceptarProc.Click
        Dim dt As New DataTable
        Dim folios As String
        Try
            If txtToken.Text = "" Then
                Mensaje.MuestraMensaje("Token", "Debe capturar número de token que llego a su correo para autorizar", TipoMsg.Advertencia)
            Else


                If hid_Token.Value = txtToken.Text Then
                    If Not Trim(txtToken.Text) = "0" Then
                        If autorizaCheques() Then
                            Funciones.CerrarModal("#Resumen")
                            folios = Session("folios")
                            Funciones.fn_Consulta("usp_resumen_aut_rech '" & folios & "'", dt)
                            grdProc.DataSource = dt
                            grdProc.DataBind()
                            Funciones.AbrirModal("#modProcesado")
                        Else
                            Funciones.CerrarModal("#Resumen")
                            Mensaje.MuestraMensaje("Cartas Cheque", "No existen datos para autorizar o rechazar, seleccione un estado", TipoMsg.Falla)
                        End If



                        Session("folios") = ""
                    Else
                        Mensaje.MuestraMensaje("Valida Token", "El código proporcionado no es válido", TipoMsg.Falla)
                    End If
                Else Mensaje.MuestraMensaje("Valida Token", "El código proporcionado no es válido", TipoMsg.Falla)

                End If

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("Excepción", ex.Message, TipoMsg.Falla)
        End Try
    End Sub

    Private Sub btnGenToken_Click(sender As Object, e As EventArgs) Handles btnGenToken.Click
        Dim flag As Boolean

        flag = fn_Token()
        If flag Then
            MuestraMensaje("Token", "Se ha enviado a su correo el número de TOKEN", TipoMsg.Confirma)
        End If

    End Sub
    Protected Sub dropEstado_SelectedIndexChanged(sender As Object, e As EventArgs)

        Dim row As GridViewRow = DirectCast(DirectCast(sender, DropDownList).NamingContainer, GridViewRow)


        Dim ddlRechazo = DirectCast(grd.Rows(row.RowIndex).FindControl("dropEstado"), DropDownList)
        Dim txtRechazo = DirectCast(grd.Rows(row.RowIndex).FindControl("txt_mot_rech"), TextBox)
        Dim lblRechazo = DirectCast(grd.Rows(row.RowIndex).FindControl("lbl_mot_rech"), Label)
        Dim chk = DirectCast(grd.Rows(row.RowIndex).FindControl("CheckBox1"), CheckBox)


        If ddlRechazo.SelectedValue = 3 Then
            txtRechazo.Visible = True
            txtRechazo.Focus()
            lblRechazo.Visible = True
            chk.Checked = True
        ElseIf ddlRechazo.SelectedValue = 2 Then
            txtRechazo.Visible = False
            lblRechazo.Visible = False
            txtRechazo.Text = ""
            chk.Checked = True
            'chk.Focus()
        Else
            txtRechazo.Visible = False
            lblRechazo.Visible = False
            txtRechazo.Text = ""
            chk.Checked = False
            'chk.Focus()
        End If
    End Sub


    Private Sub btnProcesado_Click(sender As Object, e As EventArgs) Handles btnProcesado.Click
        Response.Redirect("CartasChequeAutorizacion.aspx")
    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        txt_folio_carta.Text = ""
        txt_nro_ch_desde.Text = ""
        txt_nro_ch_hasta.Text = ""
        txt_nro_op.Text = ""
        txt_nro_op_hasta.Text = ""
        txt_nro_stro.Text = ""
        txt_cheque_a_nom.Text = ""
        chk_Todas.Checked = False
        chk_Rechazadas.Checked = False
        chk_Autorizadas.Checked = False
        chk_Pendientes.Checked = False
    End Sub
    Protected Sub chk_Todas_CheckedChanged(sender As Object, e As EventArgs) Handles chk_Todas.CheckedChanged
        VerificaRadios(TipoFiltro.Todas)
    End Sub
    Protected Sub chk_Pendientes_CheckedChanged(sender As Object, e As EventArgs) Handles chk_Pendientes.CheckedChanged
        VerificaRadios(TipoFiltro.Pendientes)
    End Sub

    Protected Sub chk_Autorizadas_CheckedChanged(sender As Object, e As EventArgs) Handles chk_Autorizadas.CheckedChanged
        VerificaRadios(TipoFiltro.Autorizadas)
    End Sub
    Protected Sub chk_Rechazadas_CheckedChanged(sender As Object, e As EventArgs) Handles chk_Rechazadas.CheckedChanged
        VerificaRadios(TipoFiltro.Rechazadas)
    End Sub
    Private Sub VerificaRadios(tipo As Integer)

        Select Case tipo

            Case 0
                If chk_Todas.Checked Then
                    chk_Pendientes.Checked = False
                    chk_Autorizadas.Checked = False
                    chk_Rechazadas.Checked = False
                End If
            Case 1
                If chk_Pendientes.Checked Then
                    chk_Todas.Checked = False
                    chk_Autorizadas.Checked = False
                    chk_Rechazadas.Checked = False
                End If
            Case 2
                If chk_Autorizadas.Checked Then
                    chk_Pendientes.Checked = False
                    chk_Todas.Checked = False
                    chk_Rechazadas.Checked = False
                End If
            Case 3
                If chk_Rechazadas.Checked Then
                    chk_Pendientes.Checked = False
                    chk_Autorizadas.Checked = False
                    chk_Todas.Checked = False
                End If
        End Select

    End Sub

End Class


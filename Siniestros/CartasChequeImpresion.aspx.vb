Imports System.Data
Imports System.Data.SqlClient
Imports Mensaje
Imports Funciones
Imports System.IO
Partial Class Siniestros_CartasChequeImpresion
    Inherits System.Web.UI.Page
    Dim oTabla As DataTable

    Private Enum TipoFiltro
        Todas = 1
        PendientesEntrega = 2
        Rechazadas = 3 'FJCP 10267 - CC - OPs Web_Cartas_Cheque
        Entregadas = 4
    End Enum


    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        btnGuardar.Visible = False
        btn_Todas.Visible = False
        btn_Ninguna.Visible = False
        btn_Imprimir.Visible = False

        If Not ValidaFiltros() Then Exit Sub

        oTabla = BuscarOP()
        'Session("dtOP") = oTabla

        If Not IsNothing(oTabla) Then
            If oTabla.Rows.Count > 0 Then
                btnGuardar.Visible = True
                btn_Todas.Visible = True
                btn_Ninguna.Visible = True
                btn_Imprimir.Visible = True

            End If
        End If
        grd.DataSource = oTabla
        grd.DataBind()


    End Sub

    Private Function BuscarOP() As DataTable
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)

        Dim fec_desde As String
        Dim fec_hasta As String

        fec_desde = Funciones.FormatearFecha(txt_fec_gen_desde.Text, Funciones.enumFormatoFecha.YYYYMMDD)
        fec_hasta = Funciones.FormatearFecha(txt_fec_gen_hasta.Text, Funciones.enumFormatoFecha.YYYYMMDD)



        Try
            oParametros.Add("folio_gmx_desde", ValidarParametros(Trim(txt_folio_desde.Text)))
            oParametros.Add("folio_gmx_hasta", ValidarParametros(Trim(txt_folio_hasta.Text)))
            oParametros.Add("fecha_desde", ValidarParametros(Trim(fec_desde)))
            oParametros.Add("fecha_hasta", ValidarParametros(Trim(fec_hasta)))
            oParametros.Add("nro_ch_desde", ValidarParametros(Trim(txt_nro_ch_desde.Text)))
            oParametros.Add("nro_ch_hasta", ValidarParametros(Trim(txt_nro_ch_hasta.Text)))
            oParametros.Add("nro_stro", ValidarParametros(Trim(txt_nro_stro.Text)))
            oParametros.Add("txt_cheque_a_nom", ValidarParametros(Trim(txt_cheque_a_nom.Text)))


            If chk_Ninguno.Checked Then
                oParametros.Add("pend_entrega", -1)
                oParametros.Add("entregadas", -1)
            Else
                oParametros.Add("pend_entrega", IIf(chk_Pendientes.Checked = True, -1, 0))
                oParametros.Add("entregadas", IIf(chk_Entregadas.Checked = True, -1, 0))
                oParametros.Add("rechazadas", IIf(chk_Rechazadas.Checked = True, -1, 0)) 'FJCP 10267 - CC - OPs Web_Cartas_Cheque
            End If


            oDatos = Funciones.ObtenerDatos("usp_buscar_datos_carta_ch_impresion", oParametros)
            oTabla = oDatos.Tables(0)



            Return oTabla

        Catch ex As Exception
            MuestraMensaje("Exception", "BuscarOP: " & ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try
    End Function


    Protected Sub chk_Pendientes_CheckedChanged(sender As Object, e As EventArgs) Handles chk_Pendientes.CheckedChanged
        VerificaRadios(TipoFiltro.PendientesEntrega)
    End Sub

    Protected Sub chk_Ninguno_CheckedChanged(sender As Object, e As EventArgs) Handles chk_Ninguno.CheckedChanged
        VerificaRadios(TipoFiltro.Todas)
    End Sub

    Protected Sub chk_Entregadas_CheckedChanged(sender As Object, e As EventArgs) Handles chk_Entregadas.CheckedChanged
        VerificaRadios(TipoFiltro.Entregadas)
    End Sub

    Protected Sub chk_Rechazadas_CheckedChanged(sender As Object, e As EventArgs) Handles chk_Rechazadas.CheckedChanged
        VerificaRadios(TipoFiltro.Rechazadas) 'FJCP 10267 - CC - OPs Web_Cartas_Cheque
    End Sub
    Private Sub VerificaRadios(tipo As Integer)

        Select Case tipo

            Case 1
                If chk_Ninguno.Checked Then
                    chk_Entregadas.Checked = False
                    chk_Pendientes.Checked = False
                    chk_Rechazadas.Checked = False 'FJCP 10267 - CC - OPs Web_Cartas_Cheque
                End If
            Case 2
                If chk_Pendientes.Checked Then
                    chk_Entregadas.Checked = False
                    chk_Ninguno.Checked = False
                    chk_Rechazadas.Checked = False 'FJCP 10267 - CC - OPs Web_Cartas_Cheque
                End If
            Case 3 'FJCP 10267 - CC - OPs Web_Cartas_Cheque
                If chk_Rechazadas.Checked Then
                    chk_Entregadas.Checked = False
                    chk_Ninguno.Checked = False
                    chk_Pendientes.Checked = False
                End If
            Case 4
                If chk_Entregadas.Checked Then
                    chk_Pendientes.Checked = False
                    chk_Ninguno.Checked = False
                    chk_Rechazadas.Checked = False 'FJCP 10267 - CC - OPs Web_Cartas_Cheque
                End If
        End Select

    End Sub


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

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Dim dtSelec As DataTable
        Dim msgError As String
        'FJCP 10267 - CC - OPs Web_Cartas_Cheque Ini
        Dim errorFolios As Boolean
        Dim foliosErrorFecha As String
        Dim foliosErrorFechaEntrega As String 'FCRUZ_GMX-10290_INCIDENCIAS
        Dim foliosErrorMotivo As String
        'FJCP 10267 - CC - OPs Web_Cartas_Cheque Fin

        'FCRUZ_GMX-10290_INCIDENCIAS INI
        Dim fechaCreacion As Date
        Dim fechaEntrega As Date
        'FCRUZ_GMX-10290_INCIDENCIAS FIN

        dtSelec = obtenerSeleccionados()
        'FJCP 10267 - CC - OPs Web_Cartas_Cheque Ini
        foliosErrorFecha = ""
        foliosErrorMotivo = ""
        foliosErrorFechaEntrega = "" 'FCRUZ_GMX-10290_INCIDENCIAS
        errorFolios = False
        msgError = ""

        If dtSelec.Rows.Count > 0 Then
            For Each row As DataRow In dtSelec.Rows
                If row("ACCION").ToString() = 3 Then
                    'If row("FECHA DE ENTREGA O RECHAZO").ToString() = "" Then
                    '    foliosErrorFecha = foliosErrorFecha & row("FOLIO CARTA").ToString() & ", "
                    'End If
                    If row("MOTIVO RECHAZO").ToString() = "" Then
                        foliosErrorMotivo = foliosErrorMotivo & row("FOLIO CARTA").ToString() & ", "
                    End If
                End If

                If row("ACCION").ToString() = 4 Then
                    If row("FECHA DE ENTREGA O RECHAZO").ToString() = "" Then
                        foliosErrorFecha = foliosErrorFecha & row("FOLIO CARTA").ToString() & ", "
                        'FCRUZ_GMX-10290_INCIDENCIAS INI
                    Else
                        fechaCreacion = row("FECHA GENERADA").ToString()
                        fechaEntrega = row("FECHA DE ENTREGA O RECHAZO").ToString()

                        If fechaCreacion > fechaEntrega Then
                            foliosErrorFechaEntrega = foliosErrorFechaEntrega & row("FOLIO CARTA").ToString() & ", "
                        End If
                        'FCRUZ_GMX-10290_INCIDENCIAS FIN
                    End If
                End If
            Next

            If foliosErrorFecha <> "" Then
                msgError = "Falta capturar fecha entrega para folio(s): " + foliosErrorFecha + "<br>"
                errorFolios = True
            End If

            If foliosErrorMotivo <> "" Then
                msgError = msgError + "Falta capturar motivo de rechazo para folio(s): " & foliosErrorMotivo
                errorFolios = True
            End If

            'FCRUZ_GMX-10290_INCIDENCIAS INI
            If foliosErrorFechaEntrega <> "" Then
                msgError = "La fecha de entrega no puede ser menor a la fecha de generación de la carta. Corregir fecha en folio(s): " + foliosErrorFechaEntrega + "<br>"
                errorFolios = True
            End If
            'FCRUZ_GMX-10290_INCIDENCIAS FIN

            If errorFolios Then
                MuestraMensaje("Fecha de Entrega", msgError, TipoMsg.Advertencia)
                Exit Sub
            End If
            'FJCP 10267 - CC - OPs Web_Cartas_Cheque Fin


            Dim dtActualiza = New DataTable
            ActualizaEntregada(dtSelec)
            dtActualiza = BuscarOP()
            grd.DataSource = dtActualiza
            grd.DataBind()
            'Funciones.AbrirModal("#ModConfirmar")

        Else
            MuestraMensaje("Validación", "No existen registros seleccionados para autorizar", TipoMsg.Advertencia)
        End If
    End Sub

    'Private Sub btnSi_Click(sender As Object, e As EventArgs) Handles btnSi.Click
    '    Dim dtSelAct As DataTable
    '    dtSelAct = Session("dtSelAct")

    '    Dim dtActualiza = New DataTable
    '    ActualizaEntregada(dtSelAct)
    '    Session("dtSelAct") = Nothing
    '    Funciones.CerrarModal("#ModConfirmar")
    '    dtActualiza = BuscarOP()
    '    grd.DataSource = dtActualiza
    '    grd.DataBind()

    'End Sub

    Private Function obtenerSeleccionados() As DataTable


        Try
            Dim dtSel As New DataTable
            Dim add As Boolean
            Dim indexTabla As Integer
            add = True
            indexTabla = 0


            If grd.Rows.Count = 0 Then
                MuestraMensaje("Validacion", "No existen registros en la tabla", TipoMsg.Advertencia)
            End If


            For Each cell As TableCell In grd.HeaderRow.Cells
                dtSel.Columns.Add(cell.Text)
            Next


            For index = 0 To grd.Rows.Count - 1
                Dim eliminar As CheckBox
                eliminar = grd.Rows(index).FindControl("Chk_Entregado")
                If eliminar.Checked Then
                    dtSel.Rows.Add()
                End If
            Next

            For Each row As GridViewRow In grd.Rows
                For i As Integer = 0 To row.Cells.Count - 1
                    Dim selecc As CheckBox
                    selecc = row.Cells(i).FindControl("Chk_Entregado")

                    Dim fecha_entrega As TextBox
                    fecha_entrega = row.Cells(i).FindControl("txt_fec_entrega")

                    'FJCP 10267 - CC - OPs Web_Cartas_Cheque Ini
                    Dim mot_rechaazo As TextBox
                    mot_rechaazo = row.Cells(i).FindControl("txt_mot_rech")

                    Dim accion As DropDownList
                    accion = row.Cells(i).FindControl("dropEstado")
                    'FJCP 10267 - CC - OPs Web_Cartas_Cheque Fin

                    If selecc.Checked Then
                        'FJCP 10267 - CC - OPs Web_Cartas_Cheque Ini
                        If i = 6 Then
                            dtSel.Rows(indexTabla)(i) = accion.SelectedValue
                            add = True
                        ElseIf i = 7 Then
                            dtSel.Rows(indexTabla)(i) = fecha_entrega.Text.ToString()
                            add = True
                        ElseIf i = 8 Then
                            dtSel.Rows(indexTabla)(i) = mot_rechaazo.Text.ToString()
                            add = True
                            'FJCP 10267 - CC - OPs Web_Cartas_Cheque Fin
                        Else
                            dtSel.Rows(indexTabla)(i) = row.Cells(i).Text
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
            MuestraMensaje("Exception", "obtenerSeleccionados: " & ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try
    End Function

    Private Sub ActualizaEntregada(dt As DataTable)
        Dim oParametros As New Dictionary(Of String, Object)

        Try

            For Each row As DataRow In dt.Rows
                oParametros = New Dictionary(Of String, Object)
                oParametros.Add("folio_carta_gmx", ValidarParametros(row("FOLIO CARTA")))
                oParametros.Add("cod_ususario", Master.cod_usuario.ToString())

                'FJCP 10267 - CC - OPs Web_Cartas_Cheque Ini
                If row("ACCION") = 3 Then
                    'oParametros.Add("fec_proceso", Funciones.FormatearFecha(row("FECHA DE ENTREGA O RECHAZO"), Funciones.enumFormatoFecha.YYYYMMDD))
                    oParametros.Add("status", 3)
                    oParametros.Add("mot_rechazo", row("MOTIVO RECHAZO").ToString())
                End If

                If row("ACCION") = 4 Then
                    oParametros.Add("fec_entrega", Funciones.FormatearFecha(row("FECHA DE ENTREGA O RECHAZO"), Funciones.enumFormatoFecha.YYYYMMDD))
                    oParametros.Add("status", 4)
                End If
                'FJCP 10267 - CC - OPs Web_Cartas_Cheque Fin

                Funciones.ObtenerDatos("usp_datos_carta_ch_autoriza", oParametros)
            Next


        Catch ex As Exception
            MuestraMensaje("Exception", "ActualizaEntregada: " & ex.Message, TipoMsg.Falla)

        End Try
    End Sub


    Private Sub btn_Imprimir_Click(sender As Object, e As EventArgs) Handles btn_Imprimir.Click
        Dim dtSelec As DataTable
        Dim folioRep As Integer

        dtSelec = obtenerSeleccionadosImp()
        Dim strFoliosRep As String = ""


        Try
            If dtSelec.Rows.Count > 0 Then
                For Each row In dtSelec.Rows
                    folioRep = Funciones.fn_Ejecuta("SELECT DISTINCT folio_carta FROM datos_carta_cheque_autoriza WHERE folio_carta_gmx = " & row(0).ToString())
                    If strFoliosRep = "" Then
                        strFoliosRep = strFoliosRep & folioRep.ToString()
                    Else
                        strFoliosRep = strFoliosRep & "," & folioRep.ToString()
                    End If
                Next

                Dim ws As New ws_Generales.GeneralesClient
                Dim server As String = ws.ObtieneParametro(Cons.TargetReport)


                server = Replace(Replace(server, "@Reporte", "RepCartasCheque"), "@Formato", "PDF") & "&folio=@folio"
                server = Replace(server, Cons.ReposSource, Cons.ReposReport)

                Funciones.EjecutaFuncion(String.Format("fn_ImprimirCartas('{0}','{1}');", server, strFoliosRep))

            Else
                MuestraMensaje("Validación", "No existen registros seleccionados para imprimir", TipoMsg.Advertencia)
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


            dtSel.Columns.Add("FOLIO_GMX")


            For index = 0 To grd.Rows.Count - 1
                Dim eliminar As CheckBox
                eliminar = grd.Rows(index).FindControl("chk_Print")
                If eliminar.Checked Then
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
            MuestraMensaje("Exception", "obtenerSeleccionados: " & ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try

    End Function

    Private Function ValidaFiltros() As Boolean
        Dim nroFolioDesde As Double
        Dim nroFolioHasta As Double
        Dim nroChequeDesde As Double
        Dim nroChequeHasta As Double
        Dim fec_desde As Date
        Dim fec_hasta As Date

        ValidaFiltros = True



        If Trim(txt_folio_desde.Text) = "" Then
            If Trim(txt_folio_hasta.Text) = "" Then
                If Trim(txt_nro_ch_desde.Text) = "" Then
                    If Trim(txt_nro_ch_hasta.Text) = "" Then
                        If Trim(txt_fec_gen_desde.Text) = "" Then
                            If Trim(txt_fec_gen_hasta.Text) = "" Then
                                If Trim(txt_nro_stro.Text) = "" Then
                                    If Trim(txt_cheque_a_nom.Text) = "" Then
                                        If chk_Entregadas.Checked = False Then
                                            If chk_Rechazadas.Checked = False Then 'FJCP 10267 - CC - OPs Web_Cartas_Cheque
                                                If chk_Pendientes.Checked = False Then
                                                    If chk_Ninguno.Checked = False Then
                                                        MuestraMensaje("Validación", "Debe elegir al menos un filtro para la consulta", TipoMsg.Advertencia)
                                                        ValidaFiltros = False
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            Else
                                MuestraMensaje("Validación", "Debe seleccionar fecha Genera desde", TipoMsg.Advertencia)
                                ValidaFiltros = False
                            End If
                        Else
                            If Not Trim(txt_fec_gen_hasta.Text) = "" Then
                                fec_desde = txt_fec_gen_desde.Text
                                fec_hasta = txt_fec_gen_hasta.Text

                                If fec_desde > fec_hasta Then
                                    MuestraMensaje("Validación", "Fecha generación desde debe ser mayor que fecha generación hasta", TipoMsg.Advertencia)
                                    ValidaFiltros = False
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
                MuestraMensaje("Validación", "Debe seleccionar Núm Folio desde", TipoMsg.Advertencia)
                ValidaFiltros = False
            End If
        Else
            If Not Trim(txt_folio_hasta.Text) = "" Then
                nroFolioDesde = Convert.ToDouble(Trim(txt_folio_desde.Text))
                nroFolioHasta = Convert.ToDouble(Trim(txt_folio_hasta.Text))

                If nroFolioDesde > nroFolioHasta Then
                    MuestraMensaje("Validación", "Número Folio desde debe ser mayor que Número de Folio hasta", TipoMsg.Advertencia)
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
    Protected Sub Chk_Entregado_CheckedChanged(sender As Object, e As EventArgs)
        Dim row As GridViewRow = DirectCast(DirectCast(sender, CheckBox).NamingContainer, GridViewRow)


        Dim chkEntregada = DirectCast(grd.Rows(row.RowIndex).FindControl("Chk_Entregado"), CheckBox)
        Dim txtFecEntregada = DirectCast(grd.Rows(row.RowIndex).FindControl("txt_fec_entrega"), TextBox)


        If chkEntregada.Checked Then
            txtFecEntregada.Enabled = True
            txtFecEntregada.Focus()
        Else
            txtFecEntregada.Enabled = False
        End If
    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        txt_folio_desde.Text = ""
        txt_folio_hasta.Text = ""
        txt_nro_ch_desde.Text = ""
        txt_nro_ch_hasta.Text = ""
        txt_fec_gen_desde.Text = ""
        txt_fec_gen_hasta.Text = ""
        txt_nro_stro.Text = ""
        txt_cheque_a_nom.Text = ""
        chk_Ninguno.Checked = False
        chk_Pendientes.Checked = False
        chk_Entregadas.Checked = False
        Response.Redirect("CartasChequeImpresion.aspx")
    End Sub

    Protected Sub dropEstado_SelectedIndexChanged(sender As Object, e As EventArgs)
        'FJCP 10267 - CC - OPs Web_Cartas_Cheque Fin
        Dim row As GridViewRow = DirectCast(DirectCast(sender, DropDownList).NamingContainer, GridViewRow)


        Dim ddlRechazo = DirectCast(grd.Rows(row.RowIndex).FindControl("dropEstado"), DropDownList)
        Dim txtRechazo = DirectCast(grd.Rows(row.RowIndex).FindControl("txt_mot_rech"), TextBox)
        Dim txtFecEntregada = DirectCast(grd.Rows(row.RowIndex).FindControl("txt_fec_entrega"), TextBox)
        Dim chkEntregada = DirectCast(grd.Rows(row.RowIndex).FindControl("Chk_Entregado"), CheckBox)




        If ddlRechazo.SelectedValue = 3 Then
            txtRechazo.Enabled = True
            txtFecEntregada.Enabled = False

            chkEntregada.Checked = True
            txtFecEntregada.Text = DateTime.Now.ToString("dd/MM/yyyy")
            txtRechazo.Focus()


        ElseIf ddlRechazo.SelectedValue = 4 Then
            txtFecEntregada.Enabled = True
            txtFecEntregada.Focus()
            txtRechazo.Enabled = False
            txtRechazo.Text = ""
            chkEntregada.Checked = True

        Else
            txtFecEntregada.Enabled = False
            txtFecEntregada.Text = ""
            txtFecEntregada.Enabled = False
            txtRechazo.Text = ""
            chkEntregada.Checked = False
            txtRechazo.Enabled = False

        End If
    End Sub

End Class


Imports System.Data
Imports System.Windows.Forms
Imports Mensaje
Imports Funciones




Partial Class Siniestros_CartasCheque

    Inherits System.Web.UI.Page
    Dim oTabla As DataTable
    Dim folio As Integer

    Private Enum TipoFiltro
        Todas = 0
        Pendientes = 1
        Elaboradas = 2

    End Enum

    Protected Sub btn_BuscarOP_Click(sender As Object, e As EventArgs) Handles btn_BuscarOP.Click

        btnSolAut.Enabled = False
        btn_Continuar.Visible = False
        limpiaTxtPresente()
        limpiaTxtEnvio()
        If Not ValidaFiltros() Then Exit Sub
        oTabla = BuscarOP()
        Session("dtOP") = oTabla

        If oTabla.Rows.Count > 0 Then btn_Continuar.Visible = True

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
            oParametros.Add("pendientes", IIf(chk_Pendientes.Checked = True, -1, 0))
            oParametros.Add("elaboradas", IIf(chk_Elaboradas.Checked = True, -1, 0))


            oDatos = Funciones.ObtenerDatos("usp_cartas_cheque_op_web", oParametros)
            oTabla = oDatos.Tables(0)

            chk_autoriza.Checked = False

            Return oTabla

        Catch ex As Exception
            MuestraMensaje("Exception", "BuscarOP: " & ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try
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


    Private Sub chkEnvio_CheckedChanged(sender As Object, e As EventArgs) Handles chkEnvio.CheckedChanged
        If chkEnvio.Checked Then
            limpiaTxtEnvio()
            txt_cc_Clave.Enabled = True
            chkPresente.Checked = False
            txtRecibe.Enabled = False
            txtEmpresaRemite.Enabled = False
            limpiaTxtPresente()
            btnSolAut.Enabled = False
        Else
            limpiaTxtEnvio()
            txt_cc_Clave.Enabled = False
            chkPresente.Checked = False
            txtRecibe.Enabled = False
            txtEmpresaRemite.Enabled = False
            limpiaTxtPresente()
        End If

    End Sub

    Private Sub chkPresente_CheckedChanged(sender As Object, e As EventArgs) Handles chkPresente.CheckedChanged
        If chkPresente.Checked Then
            limpiaTxtPresente()
            txtRecibe.Enabled = True
            txtEmpresaRemite.Enabled = True
            chkEnvio.Checked = False
            txt_cc_Clave.Enabled = False
            limpiaTxtEnvio()
            btnSolAut.Enabled = False
        Else
            limpiaTxtPresente()
            txtRecibe.Enabled = False
            txtEmpresaRemite.Enabled = False
            chkEnvio.Checked = False
            txt_cc_Clave.Enabled = False
            limpiaTxtEnvio()
        End If

    End Sub

    Private Sub limpiaTxtEnvio()
        txt_cc_Clave.Text = ""
        txtEmpresa.Text = ""
        txtCalle.Text = ""
        txtColonia.Text = ""
        txtDeleg.Text = ""
        txtEntidad.Text = ""
        txtCodPostal.Text = ""
        txtAtencion.Text = ""
    End Sub
    Private Sub limpiaTxtPresente()
        txtEmpresaRemite.Text = ""
        txtRecibe.Text = ""
    End Sub

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
                            If Trim(txt_cheque_a_nom.Text) = "" Then
                                If Not chk_Todas.Checked Then
                                    If Not chk_Pendientes.Checked Then
                                        If Not chk_Elaboradas.Checked Then
                                            MuestraMensaje("Validación", "Debe elegir al menos un filtro para la consulta", TipoMsg.Advertencia)
                                            ValidaFiltros = False
                                        End If
                                    End If
                                Else
                                    ValidaFiltros = True
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

    Public Sub txt_cc_Clave_TextChanged(sender As Object, e As EventArgs) Handles txt_cc_Clave.TextChanged

        If Not txt_cc_Clave.Text = "" Then
            datos_para_de_cartas(Convert.ToInt32(txt_cc_Clave.Text))
        Else
            limpiaTxtEnvio()
        End If
        txt_cc_Clave.Enabled = True
    End Sub

    Private Sub datos_para_de_cartas(control_clave As Integer)
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)

        Try
            oParametros.Add("clave", control_clave)

            oDatos = Funciones.ObtenerDatos("usp_datos_carta_cheque", oParametros)
            oTabla = oDatos.Tables(0)

            If Not oTabla.Rows.Count > 0 Then
                limpiaTxtEnvio()
                MuestraMensaje("Validación", "Clave Inexistente o inactiva", TipoMsg.Advertencia)

                Exit Sub
            End If

            For Each row As DataRow In oTabla.Rows

                txtEmpresa.Text = row("txt_empresa").ToString()
                txtCalle.Text = row("txt_calle").ToString()
                txtColonia.Text = row("txt_colonia").ToString()
                txtDeleg.Text = row("txt_municipio").ToString()
                txtEntidad.Text = row("txt_tdpto").ToString()
                txtCodPostal.Text = row("txt_cod_postal").ToString()
                txtAtencion.Text = row("txt_atencion").ToString()
            Next

        Catch ex As Exception
            MuestraMensaje("Valida", ex.Message, TipoMsg.Advertencia)
        End Try

    End Sub
    Private Function ValidaDatosEnvio() As Boolean
        If chkEnvio.Checked = False And chkPresente.Checked = False Then
            MuestraMensaje("Validación", "Debe seleccionar una opción: ENVÍO o PRESENTE", TipoMsg.Confirma)
            Return False

        Else
            If chkEnvio.Checked = True And Trim(txt_cc_Clave.Text) = "" Then
                MuestraMensaje("Validación", "Clave es un campo obligatorio", TipoMsg.Confirma)
                Return False
            End If

            If chkPresente.Checked = True And Trim(txtRecibe.Text) = "" Then
                MuestraMensaje("Validación", "Persona que recibe el cheque es un campo obligatorio", TipoMsg.Confirma)
                Return False
            End If

            If chkPresente.Checked = True And Trim(txtEmpresaRemite.Text) = "" Then
                MuestraMensaje("Validación", "Empresa de donde viene es un campo obligatorio", TipoMsg.Confirma)
                Return False
            End If

            Return True
        End If
    End Function


    Private Sub btnPreview_Click(sender As Object, e As EventArgs) Handles btnPreview.Click
        'Dim nrofolio As Integer
        Dim bFlag As Boolean
        bFlag = ValidaDatosEnvio()
        If bFlag = True Then

            eliminaTempPreview(hid_Folio.Value)
            hid_Folio.Value = Funciones.fn_Ejecuta("Declare @folio int  EXEC spiu_tvarias_ult_nro 0, folio_carta_cheque, @ult_nro = @folio OUTPUT Select @folio ")

            If GrabarDatosChequePrev(hid_Folio.Value) Then
                generaReporte(hid_Folio.Value)
                'Funciones.EjecutaFuncion("enabAutoriza();")
                btnSolAut.Enabled = True
            End If
        End If

    End Sub

    Private Sub eliminaTempPreview(folio As Integer)
        Dim oParametros As New Dictionary(Of String, Object)
        Try
            oParametros = New Dictionary(Of String, Object)

            oParametros.Add("proceso", 0)
            oParametros.Add("folio", folio)
            Funciones.ObtenerDatos("usp_grabar_tmp_datos_cheque", oParametros)
        Catch ex As Exception
            MuestraMensaje("Exception", "eliminaTempPreview: " & ex.Message, TipoMsg.Falla)

        End Try
    End Sub

    Private Function GrabarDatosChequePrev(folio As Integer) As Boolean
        Dim oParametros As New Dictionary(Of String, Object)
        Dim dtGrd As New DataTable

        Try

            dtGrd = Session("dtOP")

            Dim FecHoy As String
            FecHoy = DateTime.Now.ToString("yyyyMMdd")
            For Each row As DataRow In dtGrd.Rows
                oParametros = New Dictionary(Of String, Object)

                oParametros.Add("proceso", -1)
                oParametros.Add("folio", folio)
                oParametros.Add("nro_stro", ValidarParametros(row("nro_stro")))
                oParametros.Add("nro_ch", ValidarParametros(row("nro_ch")))
                oParametros.Add("txt_cheque_a_nom", ValidarParametros(row("txt_cheque_a_nom")))
                oParametros.Add("imp_total", CDbl(ValidarParametros(row("imp_total"))))
                oParametros.Add("nro_op", ValidarParametros(row("nro_op")))
                oParametros.Add("status", 0)
                oParametros.Add("fecha_creacion", FecHoy)
                oParametros.Add("clave", ValidarParametros(Trim(txt_cc_Clave.Text)))
                oParametros.Add("sn_envio", IIf(chkEnvio.Checked = True, -1, 0))
                oParametros.Add("recibe_cheque", ValidarParametros(Trim(txtRecibe.Text)))
                oParametros.Add("empresa_viene_cheque", ValidarParametros(Trim(txtEmpresaRemite.Text)))
                oParametros.Add("cod_usuario_crea", Master.cod_usuario)

                Funciones.ObtenerDatos("usp_grabar_tmp_datos_cheque", oParametros)


            Next
            Return True
        Catch ex As Exception
            MuestraMensaje("Exception", "GrabarDatosCheque: " & ex.Message, TipoMsg.Falla)
            Return False
        End Try
    End Function

    Private Sub btnSolAut_Click(sender As Object, e As EventArgs) Handles btnSolAut.Click
        Dim nrofolio As Integer
        Dim folioCartaGMX As Integer
        Dim bFlag As Boolean

        bFlag = True

        bFlag = ValidaDatosEnvio()
        eliminaTempPreview(hid_Folio.Value)
        If bFlag = True Then
            'If hid_Folio.Value = 0 Then
            nrofolio = Funciones.fn_Ejecuta("Declare @folio int  EXEC spiu_tvarias_ult_nro 0, folio_carta_cheque, @ult_nro = @folio OUTPUT Select @folio ")
            'End If


            folioCartaGMX = GrabarDatosCheque(nrofolio)
            If folioCartaGMX <> -1 Then
                generaReporte(nrofolio)
                Dim sn_envio As Boolean = solicitudAutorizacionMail(folioCartaGMX)
                If Not sn_envio Then
                    MuestraMensaje("Solicitud de Autorización", "No se pudo enviar el email, favor de contactar al Supervisor de Siniestros para solicitar la autorización de la carta con No. Folio: " & folioCartaGMX.ToString(), TipoMsg.Advertencia)
                End If
                Funciones.CerrarModal("#modGeneraCartas")
                lblFolio.Text = "Se ha generado el Folio:  " + folioCartaGMX.ToString()
                Funciones.AbrirModal("#modProcesado")
            Else
                MuestraMensaje("Carta Cheque", "No se generó la carta Cheque", TipoMsg.Advertencia)
            End If
        End If
    End Sub

    Private Function GrabarDatosCheque(folio As Integer) As Integer
        Dim oDatos As DataSet
        Dim dtResult As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim datosError As String
        Dim dtGrd As New DataTable
        Dim regError As Integer
        Dim folioCartaGMX As Integer
        Try
            datosError = ""
            regError = 0
            dtGrd = Session("dtOP")

            'For Each cell As TableCell In grd.HeaderRow.Cells
            '    dtGrd.Columns.Add(cell.Text)
            'Next

            'For Each row As GridViewRow In grd.Rows
            '    dtGrd.Rows.Add()
            '    For i As Integer = 0 To row.Cells.Count - 1
            '        dtGrd.Rows(row.RowIndex)(i) = row.Cells(i).Text
            '    Next
            'Next

            folioCartaGMX = Funciones.fn_Ejecuta("Declare @folio int  EXEC spiu_tvarias_ult_nro 0, folio_gmx_carta_cheque, @ult_nro = @folio OUTPUT Select @folio ")


            Dim FecHoy As String
            FecHoy = DateTime.Now.ToString("yyyyMMdd")
            For Each row As DataRow In dtGrd.Rows
                oParametros = New Dictionary(Of String, Object)

                oParametros.Add("proceso", -1)
                oParametros.Add("folio", folio)
                oParametros.Add("folio_carta_gmx", folioCartaGMX)
                oParametros.Add("nro_stro", ValidarParametros(row("nro_stro")))
                oParametros.Add("nro_ch", ValidarParametros(row("nro_ch")))
                oParametros.Add("txt_cheque_a_nom", ValidarParametros(row("txt_cheque_a_nom")))
                oParametros.Add("imp_total", CDbl(ValidarParametros(row("imp_total"))))
                oParametros.Add("nro_op", ValidarParametros(row("nro_op")))
                oParametros.Add("status", 1)
                oParametros.Add("fecha_creacion", FecHoy)
                oParametros.Add("clave", ValidarParametros(Trim(txt_cc_Clave.Text)))
                oParametros.Add("sn_envio", IIf(chkEnvio.Checked = True, -1, 0))
                oParametros.Add("recibe_cheque", ValidarParametros(Trim(txtRecibe.Text)))
                oParametros.Add("empresa_viene_cheque", ValidarParametros(Trim(txtEmpresaRemite.Text)))
                oParametros.Add("cod_usuario_crea", Master.cod_usuario)

                oDatos = Funciones.ObtenerDatos("usp_grabar_tmp_datos_cheque", oParametros)
                dtResult = oDatos.Tables(0)


                If CInt(dtResult.Rows(0)("SP_N_ID_ERROR").ToString()) <> 0 Then
                    If datosError = "" Then
                        datosError = "No se han grabado los siguientes datos: " & vbNewLine & "Siniestro: " & row("nro_stro").ToString() & " - Cheque: " & row("nro_ch").ToString()
                        regError = regError + 1
                    Else
                        datosError = datosError & vbNewLine & "Siniestro: " & row("nro_stro").ToString() & " - Cheque: " & row("nro_ch").ToString()
                        regError = regError + 1
                    End If
                End If
            Next

            If datosError <> "" Then
                MuestraMensaje("Exception", datosError, TipoMsg.Advertencia)
            End If

            If regError = grd.Rows.Count Then
                Return -1
                Exit Function
            End If

            Return folioCartaGMX
        Catch ex As Exception
            MuestraMensaje("Exception", "GrabarDatosCheque: " & ex.Message, TipoMsg.Falla)
            Return -1
        End Try
    End Function

    Private Function solicitudAutorizacionMail(nro_folio As Integer) As Boolean
        'Dim oDatos As DataSet
        'Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Try


            oParametros.Add("folio_carta", nro_folio)
            oParametros.Add("UsrSolicitante", Master.cod_usuario.ToString())
            oParametros.Add("url", HttpContext.Current.Request.Url.AbsoluteUri.Replace("CartasCheque", "CartasChequeAutorizacion"))
            oParametros.Add("sn_autoriza", IIf(chk_autoriza.Checked = True, -1, 0))

            Funciones.ObtenerDatos("usp_solicitud_aut_mail", oParametros)
            'oDatos = Funciones.ObtenerDatos("usp_solicitud_aut_mail", oParametros)
            'oTabla = oDatos.Tables(0)

            Return True

        Catch ex As Exception
            MuestraMensaje("Exception", "BuscarOP: " & ex.Message, TipoMsg.Falla)
            Return False
        End Try

    End Function
    Protected Sub grd_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles grd.RowDeleting

        Dim dt As New DataTable
        Try
            dt = Session("dtOP")

            dt.Rows.RemoveAt(e.RowIndex)
            'Guardo los nuevos valores          
            Session("dtOP") = dt
            grd.DataSource = dt
            grd.DataBind()
            btnSolAut.Enabled = False

        Catch ex As Exception
            MuestraMensaje("Exception", ex.Message, TipoMsg.Falla)
        End Try
    End Sub
    Private Sub generaReporte(nrofolio As Integer)
        Dim ws As New ws_Generales.GeneralesClient
        'Dim server As String = ws.ObtieneParametro(9)
        Dim server As String = ws.ObtieneParametro(3)
        Dim RptFilters As String
        RptFilters = "&folio=" & nrofolio.ToString()

        server = Replace(Replace(server, "@Reporte", "RepCartasCheque"), "@Formato", "PDF")
        'server = Replace(server, "ReportesGMX_UAT", "ReportesOPSiniestros")
        server = Replace(server, "ReportesGMX_DESA", "ReportesOPSiniestros_DESA")
        server = server & RptFilters
        Funciones.EjecutaFuncion("window.open('" & server & "');")
    End Sub

    Private Sub btnProcesado_Click(sender As Object, e As EventArgs) Handles btnProcesado.Click
        Response.Redirect("CartasCheque.aspx")
    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        txt_nro_op.Text = ""
        txt_nro_op_hasta.Text = ""
        txt_nro_ch_desde.Text = ""
        txt_nro_ch_hasta.Text = ""
        txt_nro_stro.Text = ""
        txt_cheque_a_nom.Text = ""
        chk_Elaboradas.Checked = False
        chk_Todas.Checked = False
        chk_Pendientes.Checked = False
    End Sub

    Protected Sub chk_Todas_CheckedChanged(sender As Object, e As EventArgs) Handles chk_Todas.CheckedChanged
        VerificaRadios(TipoFiltro.Todas)
    End Sub
    Protected Sub chk_Pendientes_CheckedChanged(sender As Object, e As EventArgs) Handles chk_Pendientes.CheckedChanged
        VerificaRadios(TipoFiltro.Pendientes)
    End Sub

    Protected Sub chk_Elaboradas_CheckedChanged(sender As Object, e As EventArgs) Handles chk_Elaboradas.CheckedChanged
        VerificaRadios(TipoFiltro.Elaboradas)
    End Sub

    Private Sub VerificaRadios(tipo As Integer)

        Select Case tipo

            Case 0
                If chk_Todas.Checked Then
                    chk_Pendientes.Checked = False
                    chk_Elaboradas.Checked = False
                End If
            Case 1
                If chk_Pendientes.Checked Then
                    chk_Todas.Checked = False
                    chk_Elaboradas.Checked = False

                End If
            Case 2
                If chk_Elaboradas.Checked Then
                    chk_Pendientes.Checked = False
                    chk_Todas.Checked = False

                End If

        End Select

    End Sub



End Class



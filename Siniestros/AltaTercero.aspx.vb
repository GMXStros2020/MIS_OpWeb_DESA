Imports Mensaje
Imports System.Data
Partial Class Siniestros_AltaTercero
    Inherits System.Web.UI.Page

    Dim Tercero As New Tercero()

    Public Property dtResult() As DataTable
        Get
            Return Session("dtResult")
        End Get
        Set(ByVal value As DataTable)
            Session("dtResult") = value
        End Set
    End Property

    Dim codPais As Integer
    Dim codDpto As Integer
    Dim codMunicipio As Integer

#Region "Eventos"
    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LimpiaControles()
            txt_edad.Enabled = False
            hid_Alta_Tercero.Value = 1 'VZAVALETA_10290_CC_INICIO
        End If

    End Sub


    Private Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        Dim ds As DataSet
        If Trim(txt_codTercero.Text) = "0" Then
            MuestraMensaje("Valida", "Código de Tercero inválido", TipoMsg.Advertencia)
            txt_codTercero.Text = ""
            LimpiaControles()
            btnEditar.Visible = False
        Else

            If Not txt_codTercero.Text = "" Then
                hid_codTercero.Value = txt_codTercero.Text
                ds = Tercero.ObtenerTercero(Convert.ToInt32(txt_codTercero.Text))
                cargarTercero(ds)
            Else
                MuestraMensaje("Valida", "Por favor ingrese un número de clave", TipoMsg.Advertencia)
            End If
        End If
    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        NuevoTercero()
    End Sub

    Private Sub btnEditar_Click(sender As Object, e As EventArgs) Handles btnEditar.Click
        If hid_codTercero.Value <> txt_codTercero.Text Then
            txt_codTercero.Text = hid_codTercero.Value
        End If
        txt_codTercero.Enabled = False
        habilitarGrales()

        If chkMoral.Checked Then
            txt_apMat.Enabled = False
            txt_nombres.Enabled = False
        End If

        btnGuardar.Visible = True
        btnRegresar.Visible = True
        btnBuscar.Visible = False
        btnNuevo.Visible = False
        btnEditar.Visible = False
    End Sub
    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If ValidaDatos() Then
            AsignarVariables()
            If Tercero.RFC() Then
                Funciones.AbrirModal("#ModConfirmar")
            End If
        End If
    End Sub
    Private Sub btnSi_Click(sender As Object, e As EventArgs) Handles btnSi.Click
        grabar()
    End Sub
    Private Sub btnAcepProc_Click(sender As Object, e As EventArgs) Handles btnAcepProc.Click
        Response.Redirect("AltaTercero.aspx")
    End Sub
    Protected Sub drEstado_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drEstado.SelectedIndexChanged
        Dim codDpto As Integer
        Dim codPais As Integer
        codPais = drPais.SelectedValue
        codDpto = drEstado.SelectedValue
        LlenaDDLMunicipio(codPais, codDpto)
    End Sub

    Private Sub drPais_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drPais.SelectedIndexChanged
        Dim codPais As Integer
        codPais = drPais.SelectedValue
        LlenaDDLEstado(codPais)
    End Sub
    Private Sub btnRegresar_Click(sender As Object, e As EventArgs) Handles btnRegresar.Click
        Response.Redirect("AltaTercero.aspx")
    End Sub
    'VZAVALETA_10290_CC_INICIO
    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        Response.Redirect("AltaTercero.aspx")
    End Sub
    'VZAVALETA_10290_CC_FIN

#End Region

#Region "Metodos"
    Private Sub LimpiaControles()
        chkFisica.Checked = True
        txt_apPat.Text = ""
        txt_apMat.Text = ""
        txt_nombres.Text = ""
        txt_rfc.Text = ""
        drSexo.SelectedValue = "M"
        LlenaDDLEdoCivil()
        drEdoCivil.SelectedValue = 1
        txt_fecNac.Text = ""
        txt_lugNac.Text = ""
        txt_edad.Text = ""
        txt_ocup.Text = ""
        txt_sueldo.Text = ""
        txt_telCasa.Text = ""
        txt_telTrab.Text = ""
        txt_cel.Text = ""
        txt_paren.Text = ""
        txt_nomPariente.Text = ""
        txt_parRFC.Text = ""
        txt_mail.Text = ""
        drTipoDir.SelectedValue = 1
        LlenaDDLPais()
        drPais.SelectedValue = 10
        codPais = drPais.SelectedValue
        LlenaDDLEstado(codPais)
        drEstado.SelectedValue = 0
        codDpto = drEstado.SelectedValue
        LlenaDDLMunicipio(codPais, codDpto)
        drDeleg.SelectedValue = 0
        txt_cod_postal.Text = ""
        LlenaDDLCalle()
        txtNombreRural.Text = ""
        txtNroCasa.Text = ""
        txtNroApto.Text = ""
        txtNroApto.Text = ""
    End Sub
    Public Sub grabar()
        Dim dt As New DataTable
        Dim codTerceroNvo As Integer
        Dim codTerceroAct As Integer
        AsignarVariables()
        codTerceroAct = CInt(IIf(txt_codTercero.Text = "", 0, txt_codTercero.Text))
        dt = Tercero.grabarTercero()
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

            Dim codError As Integer
            Dim msgError As String


            codTerceroNvo = dt.Rows(0)("codTercero")
            codError = dt.Rows(0)("snFlag")
            msgError = dt.Rows(0)("msg_err").ToString()


            If codError <> -1 Then
                If codTerceroNvo <> codTerceroAct Then
                    lblClaveNueva.Text = "Se ha dado de alta un nuevo tercero con código de tercero: " & codTerceroNvo.ToString()
                    Funciones.CerrarModal("#ModConfirmar")
                    Funciones.AbrirModal("#Procesado")
                Else
                    lblClaveNueva.Text = "Se han actualizado los datos del tercero con código de tercero: " & codTerceroNvo.ToString()
                    Funciones.CerrarModal("#ModConfirmar")
                    Funciones.AbrirModal("#Procesado")
                End If
            Else

                MuestraMensaje("Validación", "Error: " + msgError, TipoMsg.Advertencia)
            End If
        End If
    End Sub
    Private Sub AsignarVariables()

        Tercero.iCodigoTercero = IIf(txt_codTercero.Text.Trim = "", 0, txt_codTercero.Text.Trim)
        Tercero.sApellido1 = txt_apPat.Text.Trim
        Tercero.sApellido2 = txt_apMat.Text.Trim
        Tercero.sNombre = txt_nombres.Text.Trim
        Tercero.sNit = txt_rfc.Text.Trim

        If chkFisica.Checked Then
            Tercero.sTipoPersona = "F"
            Tercero.sSexo = drSexo.SelectedValue


            Tercero.sLugarNacim = txt_lugNac.Text.Trim
            Tercero.iOcupacion = -1
            Tercero.dSueldo = IIf(txt_sueldo.Text.Trim = "", -1, txt_sueldo.Text.Trim)
            Tercero.iEdad = txt_edad.Text.Trim
        ElseIf chkMoral.Checked Then
            Tercero.sTipoPersona = "J"
            Tercero.sSexo = "F"
            Tercero.sLugarNacim = ""
            Tercero.iOcupacion = 0
            Tercero.dSueldo = 0
            Tercero.iEdad = txt_edad.Text.Trim
        End If
        Tercero.iEstCivil = drEdoCivil.SelectedValue
        Tercero.vFecNacim = Funciones.FormatearFecha(txt_fecNac.Text.Trim, Funciones.enumFormatoFecha.YYYYMMDD)
        Tercero.sOcupacion = txt_ocup.Text.Trim
        Tercero.sParentesco = txt_paren.Text.Trim
        Tercero.sNombrePariente = txt_nomPariente.Text.Trim
        Tercero.sNroSSPariente = txt_parRFC.Text.Trim
        'Direccion
        Tercero.iTipoDir = drTipoDir.SelectedValue
        Tercero.icodPais = drPais.SelectedValue
        Tercero.icodDpto = drEstado.SelectedValue
        Tercero.icodMun = drDeleg.SelectedValue
        Tercero.scodPostal = txt_cod_postal.Text.Trim
        Tercero.icodCalle1 = drCalle.SelectedValue
        Tercero.sBarrio = txtNombreRural.Text.Trim
        Tercero.sNroCasa = IIf(txtNroCasa.Text.Trim = "", 0, txtNroCasa.Text.Trim)
        Tercero.sNomCalle = txtNomCalle.Text.Trim
        Tercero.sOfic = txtNroApto.Text.Trim
        'Telefonos
        Tercero.sTelCasa = txt_telCasa.Text.Trim
        Tercero.sTelTrab = txt_telTrab.Text.Trim
        Tercero.sCel = txt_cel.Text.Trim
        Tercero.sEmail = txt_mail.Text.Trim
        Tercero.sCodUsuario = Master.cod_usuario.ToString()
        Tercero.sOrigen = "T"

    End Sub
    Private Sub LlenaDDLEdoCivil()
        Funciones.fn_Consulta("sp_Catalogos_OPMasivas @strCatalogo = 'EdoCivil'", dtResult)
        Funciones.LlenaDDL(drEdoCivil, dtResult, "cod_est_civil", "txt_desc", 0, False)
    End Sub

    Private Sub LlenaDDLCalle()
        Funciones.fn_Consulta("sp_Catalogos_OPMasivas @strCatalogo = 'tcalle'", dtResult)
        Funciones.LlenaDDL(drCalle, dtResult, "cod_calle", "txt_desc", 0, False)
    End Sub

    Private Sub LlenaDDLPais()
        Dim dt As New DataTable
        Funciones.fn_Consulta("usp_obtener_cat_dir @catalogo = 'Pais'", dtResult)
        'dt = agregaSeleccionar(dtResult)
        Funciones.LlenaDDL(drPais, dtResult, "cod_pais", "txt_desc", 0, False)
    End Sub

    Private Sub LlenaDDLEstado(codPais As Integer)
        Dim dt As New DataTable
        Funciones.fn_Consulta("usp_obtener_cat_dir @catalogo = 'Estado', @cod_pais = " & codPais.ToString(), dtResult)
        'dt = agregaSeleccionar(dtResult)
        Funciones.LlenaDDL(drEstado, dtResult, "cod_dpto", "txt_desc", 0, False)
    End Sub

    Private Sub LlenaDDLMunicipio(codPais As Integer, codDpto As Integer)

        Dim dt As New DataTable
        Funciones.fn_Consulta("usp_obtener_cat_dir @catalogo = 'Municipio', @cod_dpto = " & codDpto.ToString() & ", @cod_pais = " & codPais.ToString(), dtResult)
        'dt = agregaSeleccionar(dtResult)
        Funciones.LlenaDDL(drDeleg, dtResult, "cod_municipio", "txt_desc", 0, False)

    End Sub
    Private Sub cargarTercero(ds As DataSet)
        Dim tpersona As String
        Try
            If Not ds Is Nothing AndAlso ds.Tables(0).Rows.Count > 0 Then

                tpersona = ds.Tables(0).Rows(0).Item("cod_tipo_persona").ToString.Trim
                If tpersona = "F" Then
                    chkFisica.Checked = True
                    chkMoral.Checked = False
                    chkFisica.Enabled = False
                    chkMoral.Enabled = False
                Else
                    chkMoral.Checked = True
                    chkFisica.Checked = False
                    chkFisica.Enabled = False
                    chkMoral.Enabled = False
                End If

                txt_apPat.Text = ds.Tables(0).Rows(0).Item("txt_apellido1").ToString.Trim
                txt_apMat.Text = ds.Tables(0).Rows(0).Item("txt_apellido2").ToString.Trim
                txt_nombres.Text = ds.Tables(0).Rows(0).Item("txt_nombre").ToString.Trim
                txt_rfc.Text = ds.Tables(0).Rows(0).Item("nro_nit").ToString.Trim
                drSexo.SelectedValue = ds.Tables(0).Rows(0).Item("txt_sexo")
                drEdoCivil.SelectedValue = ds.Tables(0).Rows(0).Item("cod_est_civil")
                txt_fecNac.Text = ds.Tables(0).Rows(0).Item("fec_nac").ToString.Trim
                txt_lugNac.Text = ds.Tables(0).Rows(0).Item("txt_lugar_nac").ToString.Trim

                txt_edad.Text = ds.Tables(1).Rows(0).Item("nro_edad").ToString.Trim
                txt_ocup.Text = ds.Tables(1).Rows(0).Item("txt_ocupacion").ToString.Trim
                txt_sueldo.Text = ds.Tables(1).Rows(0).Item("imp_sueldo").ToString.Trim
                txt_paren.Text = ds.Tables(1).Rows(0).Item("txt_parentesco").ToString.Trim
                txt_nomPariente.Text = ds.Tables(1).Rows(0).Item("txt_nom_pariente").ToString.Trim
                txt_parRFC.Text = ds.Tables(1).Rows(0).Item("txt_ss_pariente").ToString.Trim

                txt_telCasa.Text = ds.Tables(2).Rows(0).Item("txt_telCasa").ToString.Trim
                txt_telTrab.Text = ds.Tables(2).Rows(0).Item("txt_telTrab").ToString.Trim
                txt_mail.Text = ds.Tables(2).Rows(0).Item("txt_email").ToString.Trim
                txt_cel.Text = ds.Tables(2).Rows(0).Item("txt_cel").ToString.Trim

                drTipoDir.SelectedValue = ds.Tables(3).Rows(0).Item("cod_tipo_dir")
                drPais.SelectedValue = ds.Tables(3).Rows(0).Item("cod_pais")
                LlenaDDLEstado(drPais.SelectedValue)
                drEstado.SelectedValue = ds.Tables(3).Rows(0).Item("cod_dpto")
                LlenaDDLMunicipio(drPais.SelectedValue, drEstado.SelectedValue)
                drDeleg.SelectedValue = ds.Tables(3).Rows(0).Item("cod_municipio")
                txt_cod_postal.Text = ds.Tables(3).Rows(0).Item("nro_cod_postal").ToString.Trim
                drCalle.SelectedValue = ds.Tables(3).Rows(0).Item("cod_calle1")
                txtNombreRural.Text = ds.Tables(3).Rows(0).Item("barrio").ToString.Trim
                txtNroCasa.Text = ds.Tables(3).Rows(0).Item("nro_casa").ToString.Trim
                txtNomCalle.Text = ds.Tables(3).Rows(0).Item("nombre_calle").ToString.Trim
                txtNroApto.Text = ds.Tables(3).Rows(0).Item("apto").ToString.Trim
                btnEditar.Visible = True
            Else
                MuestraMensaje("Valida", "Código Tercero inexistente", TipoMsg.Advertencia)
                LimpiaControles()
                btnEditar.Visible = False
            End If


        Catch ex As Exception
            MuestraMensaje("Valida", ex.Message, TipoMsg.Advertencia)
        End Try

    End Sub
    Public Sub NuevoTercero()
        LimpiaControles()
        txt_codTercero.Text = ""
        txt_codTercero.Enabled = False
        habilitarGrales()
        chkFisica.Enabled = True
        chkMoral.Enabled = True
        chkMoral.Checked = False

        btnGuardar.Visible = True
        btnRegresar.Visible = True
        lblCodTercero.Visible = False
        txt_codTercero.Visible = False
        btnBuscar.Visible = False
        btnNuevo.Visible = False
        btnEditar.Visible = False
    End Sub
    Private Sub habilitarGrales()
        txt_apPat.Enabled = True
        txt_apMat.Enabled = True
        txt_nombres.Enabled = True
        txt_rfc.Enabled = True
        drSexo.Enabled = True
        drEdoCivil.Enabled = True
        txt_fecNac.Enabled = True
        txt_lugNac.Enabled = True
        txt_ocup.Enabled = True
        txt_sueldo.Enabled = True
        txt_telCasa.Enabled = True
        txt_telTrab.Enabled = True
        txt_cel.Enabled = True
        txt_paren.Enabled = True
        txt_nomPariente.Enabled = True
        txt_parRFC.Enabled = True
        txt_mail.Enabled = True
        drTipoDir.Enabled = True
        drPais.Enabled = True
        drEstado.Enabled = True
        drDeleg.Enabled = True
        txt_cod_postal.Enabled = True
        drCalle.Enabled = True
        txtNombreRural.Enabled = True
        txtNroCasa.Enabled = True
        txtNomCalle.Enabled = True
        txtNroApto.Enabled = True
    End Sub
#End Region

#Region "Funciones"
    Private Function ValidaDatos() As Boolean
        Dim msgError As String
        msgError = ""

        If chkFisica.Checked Then
            If Trim(txt_apPat.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Apellido Paterno"
            If Trim(txt_apMat.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Apellido Materno "
            If Trim(txt_nombres.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Nombres "
            If Trim(txt_fecNac.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Fecha Nacimiento "
            If Trim(txt_rfc.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "RFC "
        Else
            If Trim(txt_apPat.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Razón Social"
            If Trim(txt_fecNac.Text) = "" AndAlso Trim(txt_edad.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Fecha Nacimiento "
            If Trim(txt_rfc.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "RFC "
        End If


        If msgError <> "" Then
            MuestraMensaje("Valida campos", "Los campos " & msgError & "no puedes estar vacíos", TipoMsg.Advertencia)
            Return False
        End If
        Return True
    End Function
#End Region

End Class


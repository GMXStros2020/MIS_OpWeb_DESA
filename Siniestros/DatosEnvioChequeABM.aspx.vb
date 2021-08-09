Imports Mensaje
Imports System.Data
Partial Class Siniestros_DatosEnvioChequeABM
    Inherits System.Web.UI.Page


    Public Property dtResult() As DataTable
        Get
            Return Session("dtResult")
        End Get
        Set(ByVal value As DataTable)
            Session("dtResult") = value
        End Set
    End Property


    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            LlenaDDLEstado()
            Session("codPostalS") = 0
            CargarGrid() 'VZAVALETA_10290_Incidencias
        End If

    End Sub

    Private Sub LlenaDDLEstado()
        Dim dt As New DataTable
        Funciones.fn_Consulta("usp_obtener_cat_direccion @catalogo = 'Estado'", dtResult)
        dt = agregaSeleccionar(dtResult)
        Funciones.LlenaDDL(drEstado, dt, "cod_dpto", "txt_desc", 0, False)

    End Sub

    Private Function agregaSeleccionar(dt As DataTable) As DataTable
        Try
            Dim dtN As New DataTable

            For Each col As DataColumn In dt.Columns
                dtN.Columns.Add(col.ColumnName, col.DataType)
            Next
            dtN.Rows.Add(-1, "---SELECCIONAR---")
            dtN.Merge(dt)

            Return dtN
        Catch ex As Exception
            Mensaje.MuestraMensaje("Exepcion", ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try
    End Function
    Private Sub grabar()
        Dim objDestinatario As New DestinatariosCheque()
        Dim ClaveNueva As Integer
        Dim ClaveActual As Integer

        If Trim(txt_telefono.Text) = "" Then txt_telefono.Text = "TELEFONO NO REGISTRADO"
        ClaveActual = CInt(IIf(txt_clave.Text = "", Nothing, txt_clave.Text))
        objDestinatario.Clave = txt_clave.Text
        objDestinatario.Txt_atencion = txt_nombre.Text
        objDestinatario.Txt_calle = txt_calle.Text
        objDestinatario.Txt_empresa = txt_empresa.Text
        objDestinatario.Txt_cod_postal = txt_cod_postal.Text
        objDestinatario.Txt_telefonos = txt_telefono.Text

        objDestinatario.Cod_dpto = drEstado.SelectedValue
        objDestinatario.Cod_ciudad = drCiudad.SelectedValue
        objDestinatario.Cod_municipio = drDeleg.SelectedValue
        objDestinatario.Cod_colonia = drColonia.SelectedValue
        objDestinatario.Activo = IIf(chkActivo.Checked, -1, 0)

        ClaveNueva = objDestinatario.Save()

        If ClaveNueva <> -1 Then
            If ClaveNueva <> ClaveActual Then
                lblClaveNueva.Text = "Se ha dado de alta un nuevo registro con clave: " & ClaveNueva.ToString()
                Funciones.CerrarModal("#ModConfirmar")
                Funciones.AbrirModal("#Procesado")
            Else
                lblClaveNueva.Text = "Se han actualizado los datos del registro con clave: " & ClaveNueva.ToString()
                Funciones.CerrarModal("#ModConfirmar")
                Funciones.AbrirModal("#Procesado")
            End If
            CargarGrid() 'VZAVALETA_10290_Incidencias
        Else
            MuestraMensaje("Validación", "No fue posible grabar o actualizar los datos", TipoMsg.Advertencia)
        End If
    End Sub

    Protected Sub drEstado_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drEstado.SelectedIndexChanged
        Dim codDpto As Integer
        codDpto = drEstado.SelectedValue
        LlenaDDLCiudad(codDpto)
    End Sub

    Protected Sub drCiudad_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drCiudad.SelectedIndexChanged
        Dim codDpto As Integer
        codDpto = drEstado.SelectedValue
        LlenaDDLMunicipio(codDpto)
    End Sub

    Private Sub LlenaDDLCiudad(codDpto As Integer)

        Dim dt As New DataTable
        Funciones.fn_Consulta("usp_obtener_cat_direccion @catalogo = 'Ciudad', @cod_dpto = " & codDpto.ToString(), dtResult)
        dt = agregaSeleccionar(dtResult)
        Funciones.LlenaDDL(drCiudad, dt, "cod_ciudad", "txt_desc", 0, False)
    End Sub

    Private Sub LlenaDDLMunicipio(codDpto As Integer)

        Dim dt As New DataTable
        Funciones.fn_Consulta("usp_obtener_cat_direccion @catalogo = 'Municipio', @cod_dpto = " & codDpto.ToString(), dtResult)
        dt = agregaSeleccionar(dtResult)
        Funciones.LlenaDDL(drDeleg, dt, "cod_municipio", "txt_desc", 0, False)

    End Sub
    Protected Sub drDeleg_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drDeleg.SelectedIndexChanged
        Dim codDpto As Integer
        Dim codCiudad As Integer
        Dim codMunicipio As Integer
        codDpto = drEstado.SelectedValue
        codCiudad = drCiudad.SelectedValue
        codMunicipio = drDeleg.SelectedValue
        LlenaDDLColonia(codDpto, codCiudad, codMunicipio, Nothing)
    End Sub
    Private Sub LlenaDDLColonia(codDpto As Integer, codCiudad As Integer, codMunicipio As Integer, codPostal As String)
        Dim dt As New DataTable
        Dim sp As String

        sp = ""
        sp = sp & "usp_obtener_cat_direccion @catalogo = 'Colonia', @cod_dpto = " & codDpto.ToString() & ", @cod_ciudad = " & codCiudad.ToString() & ", @cod_municipio = " & codMunicipio.ToString()
        If Not IsNothing(codPostal) Then
            sp = sp & ", @cod_postal = '" & codPostal & "'"
        End If

        Funciones.fn_Consulta(sp, dtResult)

        dt = agregaSeleccionar(dtResult)
        Funciones.LlenaDDL(drColonia, dt, "cod_colonia", "txt_desc", 0, False)
    End Sub

    Private Sub ObtenerCP()
        Dim cod_colonia As Integer
        Dim codPostal As String

        codPostal = ""
        cod_colonia = drColonia.SelectedValue

        'codPostal = Funciones.fn_EjecutaStr("usp_obtener_cat_direccion @catalogo = 'CodPostal', @cod_colonia = " & cod_colonia.ToString())

        codPostal = Funciones.fn_EjecutaStr("usp_obtener_cat_direccion @catalogo = 'CodPostal', @cod_colonia = " & cod_colonia.ToString() & ", @cod_dpto = " & drEstado.SelectedValue & ", @cod_ciudad = " & drCiudad.SelectedValue & ", @cod_municipio = " & drDeleg.SelectedValue
        )
        'For Each row As DataRow In dtResult.Rows
        '    If row("cod_colonia") = cod_colonia Then
        '        codPostal = row("cod_postal").ToString()
        '    End If
        'Next
        ''fjcp falta corregir
        txt_cod_postal.Text = codPostal
    End Sub

    Protected Sub drColonia_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drColonia.SelectedIndexChanged
        'If Session("codPostalS") = 0 Then
        'ObtenerCP()
        'Else
        Dim a As String
        a = drColonia.SelectedItem.ToString()
        cargarDDLxColonia(drColonia.SelectedValue, txt_cod_postal.Text.Trim, drColonia.SelectedItem.ToString())
        'MuestraMensaje("s", "ffffff", TipoMsg.Advertencia)
        '   Session("codPostalS") = 0
        'End If

    End Sub

    Protected Sub txt_cod_postal_TextChanged(sender As Object, e As EventArgs) Handles txt_cod_postal.TextChanged
        Dim cod_postal As String
        cod_postal = Trim(txt_cod_postal.Text)
        Dim dt As New DataTable

        If cod_postal <> "" Then
            'Dim cod_pais As Integer
            'Funciones.fn_Consulta("usp_obtener_cat_direccion @catalogo = 'RecuperaTodos', @cod_postal = '" & cod_postal & "'", dtResult)
            Funciones.fn_Consulta("usp_obtener_cat_direccion @catalogo = 'RecuperaColonias', @cod_postal = '" & cod_postal & "'", dtResult)

            If dtResult.Rows.Count > 0 Then
                dt = agregaSeleccionar(dtResult)
                Funciones.LlenaDDL(drColonia, dt, "cod_colonia", "txt_desc", 0, False)

                'Funciones.LlenaDDL(drColonia, dtResult, "cod_colonia", "txt_desc", 0, False)

                Session("codPostalS") = 1
                drColonia.SelectedValue = -1
                drEstado.SelectedValue = -1
                drCiudad.SelectedValue = -1
                drDeleg.SelectedValue = -1
            Else
                MuestraMensaje("Valida CP", "Las colonias con el C.P. ingresado no están actualmente activas", TipoMsg.Advertencia)
                drDeleg.SelectedValue = -1
                drColonia.SelectedValue = -1
            End If
        End If

    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        LimpiaControles()
        txt_clave.Text = ""
        txt_clave.Enabled = False
        txt_nombre.Enabled = True
        txt_empresa.Enabled = True
        drEstado.Enabled = True
        drCiudad.Enabled = True
        drDeleg.Enabled = True
        drColonia.Enabled = True
        txt_calle.Enabled = True
        txt_cod_postal.Enabled = True
        txt_telefono.Enabled = True
        chkActivo.Enabled = True
        chkActivo.Checked = True
        btnGuardar.Visible = True

        btnRegresar.Visible = True
        lblClave.Visible = False
        txt_clave.Visible = False
        btnBuscar.Visible = False
        btnExportar.Visible = False
        btnNuevo.Visible = False
        btnEditar.Visible = False
    End Sub

    Private Sub btnRegresar_Click(sender As Object, e As EventArgs) Handles btnRegresar.Click
        Response.Redirect("DatosEnvioChequeABM.aspx")
    End Sub

    Private Sub btnEditar_Click(sender As Object, e As EventArgs) Handles btnEditar.Click
        If hid_clave.Value <> txt_clave.Text Then
            txt_clave.Text = hid_clave.Value
        End If

        txt_clave.Enabled = False
        txt_nombre.Enabled = True
        txt_empresa.Enabled = True
        drEstado.Enabled = True
        drCiudad.Enabled = True
        drDeleg.Enabled = True
        drColonia.Enabled = True
        txt_calle.Enabled = True
        txt_cod_postal.Enabled = True
        txt_telefono.Enabled = True
        chkActivo.Enabled = True
        btnGuardar.Visible = True

        btnRegresar.Visible = True
        'lblClave.Visible = False
        'txt_clave.Visible = False
        btnBuscar.Visible = False
        btnExportar.Visible = False
        btnNuevo.Visible = False
        btnEditar.Visible = False
    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click

        If ValidaCampos() Then
            Funciones.AbrirModal("#ModConfirmar")
        End If
    End Sub

    Private Sub btnSi_Click(sender As Object, e As EventArgs) Handles btnSi.Click
        grabar()
    End Sub

    Private Function ValidaCampos() As Boolean
        Dim msgError As String
        msgError = ""
        If Trim(txt_nombre.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Nombre Completo "
        If Trim(txt_empresa.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Empresa "

        If drEstado.SelectedValue.ToString() = "-1" Then msgError = msgError & IIf(msgError = "", "", ",") & "Estado "
        If drCiudad.SelectedValue.ToString() = "-1" Or drCiudad.SelectedValue = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Ciudad "
        If drDeleg.SelectedValue.ToString() = "-1" Or drDeleg.SelectedValue = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Deleg. o Mun. "
        If drColonia.SelectedValue.ToString() = "-1" Or drColonia.SelectedValue = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Colonia "

        If Trim(txt_calle.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Calle "
        If Trim(txt_cod_postal.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "C.P. "

        If msgError <> "" Then
            MuestraMensaje("Valida campos", "Los campos " & msgError & "no puedes estar vacíos", TipoMsg.Advertencia)
            Return False
        End If
        Return True
    End Function
    Private Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        If Trim(txt_clave.Text) = "0" Then
            MuestraMensaje("Valida", "Clave 0 inválida", TipoMsg.Advertencia)
            txt_clave.Text = ""
            LimpiaControles()
            btnEditar.Visible = False
        Else

            If Not txt_clave.Text = "" Then
                hid_clave.Value = txt_clave.Text
                obtenerDestinatario(Convert.ToInt32(txt_clave.Text))
            Else
                MuestraMensaje("Valida", "Por favor ingrese un número de clave", TipoMsg.Advertencia)
            End If
        End If
    End Sub

    Private Sub LimpiaControles()
        txt_nombre.Text = ""
        txt_empresa.Text = ""
        drEstado.SelectedValue = -1
        drCiudad.SelectedValue = -1
        drDeleg.SelectedValue = -1
        drColonia.SelectedValue = -1
        txt_calle.Text = ""
        txt_cod_postal.Text = ""
        txt_telefono.Text = ""
        chkActivo.Checked = True
    End Sub
    'VZAVALETA_10290_Incidencias_INI
    Private Sub CargarGrid()
        Dim oDatos As DataSet
        Dim oTabla As DataTable

        oDatos = Funciones.ObtenerDatos("usp_cat_destinatarios_envio")
        oTabla = oDatos.Tables(0)
        grd.DataSource = oTabla
        grd.DataBind()
    End Sub
    'VZAVALETA_10290_Incidencias_FIN

    Private Sub obtenerDestinatario(clave As Integer)
        Dim cod_dpto As Integer
        Dim cod_municipio As Integer
        Dim cod_ciudad As Integer
        Dim cod_colonia As Integer
        Dim activo As Integer
        Try
            Funciones.fn_Consulta("usp_datos_carta_cheque @clave = " & clave & ", @proceso = 1", dtResult)

            If dtResult.Rows.Count > 0 Then

                txt_empresa.Text = dtResult.Rows(0)("txt_empresa").ToString()
                txt_calle.Text = dtResult.Rows(0)("txt_calle").ToString()

                txt_cod_postal.Text = dtResult.Rows(0)("txt_cod_postal").ToString()
                txt_nombre.Text = dtResult.Rows(0)("txt_atencion").ToString()
                txt_telefono.Text = dtResult.Rows(0)("txt_telefonos").ToString()
                activo = dtResult.Rows(0)("sn_activo")

                cod_dpto = dtResult.Rows(0)("cod_dpto")
                cod_municipio = dtResult.Rows(0)("cod_municipio")
                cod_ciudad = dtResult.Rows(0)("cod_ciudad")
                cod_colonia = dtResult.Rows(0)("cod_colonia")

                drEstado.SelectedValue = cod_dpto
                LlenaDDLCiudad(cod_dpto)
                drCiudad.SelectedValue = cod_ciudad
                LlenaDDLMunicipio(cod_dpto)
                drDeleg.SelectedValue = cod_municipio
                LlenaDDLColonia(cod_dpto, cod_ciudad, cod_municipio, Nothing)
                drColonia.SelectedValue = cod_colonia
                If activo = -1 Then
                    chkActivo.Checked = True
                Else
                    chkActivo.Checked = False
                End If
                btnEditar.Visible = True
            Else
                MuestraMensaje("Valida", "Clave inexistente", TipoMsg.Advertencia)
                LimpiaControles()
                btnEditar.Visible = False
            End If


        Catch ex As Exception
            MuestraMensaje("Valida", ex.Message, TipoMsg.Advertencia)
        End Try

    End Sub

    Private Sub btnExportar_Click(sender As Object, e As EventArgs) Handles btnExportar.Click
        generaReporte()
    End Sub

    Private Sub generaReporte()
        Dim ws As New ws_Generales.GeneralesClient
        Dim server As String = ws.ObtieneParametro(Cons.TargetReport)
        Dim Random As New Random()
        Dim numero As Integer = Random.Next(1, 1000)

        Dim RptFilters As String
        RptFilters = "&numero=" & numero.ToString()
        server = Replace(Replace(server, "@Reporte", "CatDestEnvioCheque"), "@Formato", "EXCEL")
        server = Replace(server, Cons.ReposSource, Cons.ReposReport)
        server = server & RptFilters
        Funciones.EjecutaFuncion("window.open('" & server & "');")

    End Sub

    Private Sub btnAcepProc_Click(sender As Object, e As EventArgs) Handles btnAcepProc.Click
        Response.Redirect("DatosEnvioChequeABM.aspx")
    End Sub

    Private Sub cargarDDLxColonia(codColonia As Integer, codPostal As String, colDesc As String)

        Dim cod_dpto As Integer
        Dim cod_municipio As Integer
        Dim cod_ciudad As Integer
        Dim cod_colonia As Integer

        Funciones.fn_Consulta("usp_obtener_cat_direccion @catalogo = 'ColoniaSel', @cod_colonia = " & codColonia & " ,@cod_postal = '" & codPostal & "', @coloniaDesc = '" & colDesc & "'", dtResult)


        cod_dpto = dtResult.Rows(0)("cod_dpto")
        cod_municipio = dtResult.Rows(0)("cod_municipio")
        cod_ciudad = dtResult.Rows(0)("cod_ciudad")
        cod_colonia = dtResult.Rows(0)("cod_colonia")


        drEstado.SelectedValue = cod_dpto
        LlenaDDLCiudad(cod_dpto)
        drCiudad.SelectedValue = cod_ciudad
        LlenaDDLMunicipio(cod_dpto)
        drDeleg.SelectedValue = cod_municipio
        'LlenaDDLColonia(cod_dpto, cod_ciudad, cod_municipio, codPostal)
        'drColonia.SelectedValue = cod_colonia
    End Sub

    'Protected Sub txt_clave_TextChanged(sender As Object, e As EventArgs) Handles txt_clave.TextChanged
    '    If Trim(txt_clave.Text) = "0" Then
    '        txt_clave.Text = ""
    '    End If
    '    'If Trim(txt_clave.Text) = "0" Then
    '    '    txt_clave.Text = ""
    '    'Else
    '    '    If hid_clave.Value <> 0 Then
    '    '        If Not hid_clave.Value = txt_clave.Text Then
    '    '            Funciones.AbrirModal("#ModCambioClave")
    '    '        End If

    '    '    End If
    '    'End If

    'End Sub

    'Private Sub btnSi_Click(sender As Object, e As EventArgs) Handles btnSi.Click
    '    hid_clave.Value = txt_clave.Text
    '    obtenerDestinatario(Convert.ToInt32(txt_clave.Text))
    'End Sub

    'Private Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
    '    Funciones.CerrarModal("#ModCambioClave")
    '    txt_clave.Text = hid_clave.Value
    'End Sub
End Class

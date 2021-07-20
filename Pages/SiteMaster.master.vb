Imports System.Data
Imports Mensaje
Imports System.Data.SqlClient

Partial Class Pages_SiteMaster
    Inherits System.Web.UI.MasterPage
    Private dt_Consulta As DataTable
    Private dt_Datos As DataTable
    Private config_poliza() As String
    Private DetalleUsuario() As String
    Public Event _btn_ConfirmaMail_Click()
    Dim Tercero As New Tercero()

    Private Enum Filtros
        Poliza = 0
        Capa
        Ramo
        Contrato
        Compañia
        Cuota
    End Enum

    Private Enum Operacion
        Ninguna
        Consulta
        Modifica
        Anula
    End Enum

    Public Property dtPolizas() As DataTable
        Get
            Return Session("dtPolizas")
        End Get
        Set(ByVal value As DataTable)
            Session("dtPolizas") = value
        End Set
    End Property

    Public Property dtDetalleOP() As DataTable
        Get
            Return Session("dtDetalleOP")
        End Get
        Set(ByVal value As DataTable)
            Session("dtDetalleOP") = value
        End Set
    End Property

    Public Property dtContabilidad() As DataTable
        Get
            Return Session("dtContabilidad")
        End Get
        Set(ByVal value As DataTable)
            Session("dtContabilidad") = value
        End Set
    End Property

    Public Property EstadoOP() As Integer
        Get
            Return Session("EstadoOP")
        End Get
        Set(ByVal value As Integer)
            Session("EstadoOP") = value
        End Set
    End Property

    Public ReadOnly Property Contenedor() As ContentPlaceHolder
        Get
            Return cph_principal
        End Get
    End Property


    Public Property Menu() As String
        Get
            Return menu_principal.InnerHtml
        End Get
        Set(ByVal value As String)
            menu_principal.InnerHtml = value
        End Set
    End Property

    Public Property cod_usuario() As String
        Get
            Return hid_codUsuario.Value
        End Get
        Set(ByVal value As String)
            hid_codUsuario.Value = value
        End Set
    End Property

    Public Property usuario() As String
        Get
            Return lbl_Usuario.Text
        End Get
        Set(ByVal value As String)
            lbl_Usuario.Text = value
        End Set
    End Property

    Public Property cod_suc() As Integer
        Get
            Return hid_codSuc.Value
        End Get
        Set(ByVal value As Integer)
            hid_codSuc.Value = value
        End Set
    End Property

    Public Property sucursal() As String
        Get
            Return lbl_Sucursal.Text
        End Get
        Set(ByVal value As String)
            lbl_Sucursal.Text = value
        End Set
    End Property

    Public Property cod_sector() As Integer
        Get
            Return hid_codSector.Value
        End Get
        Set(ByVal value As Integer)
            hid_codSector.Value = value
        End Set
    End Property

    Public Property sector() As String
        Get
            Return lbl_Sector.Text
        End Get
        Set(ByVal value As String)
            lbl_Sector.Text = value
        End Set
    End Property

    Public Property email() As String
        Get
            Return hid_mail.Value
        End Get
        Set(ByVal value As String)
            hid_mail.Value = value
        End Set
    End Property

    Public Property user() As String
        Get
            Return hid_user.Value
        End Get
        Set(ByVal value As String)
            hid_user.Value = value
        End Set
    End Property

    Public Property pws() As String
        Get
            Return hid_pass.Value
        End Get
        Set(ByVal value As String)
            hid_pass.Value = value
        End Set
    End Property


    Public Property url_reportes() As String
        Get
            Return hid_Reportes.Value
        End Get
        Set(ByVal value As String)
            hid_Reportes.Value = value
        End Set
    End Property

    Public Property Titulo() As String
        Get
            Return lbl_TituloSeccion.Text
        End Get
        Set(ByVal value As String)
            lbl_TituloSeccion.Text = value
        End Set
    End Property

    'Public WriteOnly Property Clase_Logo() As String
    '    Set(ByVal value As String)
    '        div_Logo.Attributes("class") = value
    '    End Set
    'End Property

    Public WriteOnly Property Clase_Form() As String
        Set(ByVal value As String)
            div_Form.Attributes("class") = value
        End Set
    End Property

    'Public WriteOnly Property fecha_visible() As Boolean
    '    Set(ByVal value As Boolean)
    '        cuadro_fecha.Visible = value
    '    End Set
    'End Property

    Public Property mSeleccionados() As String
        Get
            Return hid_Seleccion.Value
        End Get
        Set(ByVal value As String)
            hid_Seleccion.Value = value
        End Set
    End Property

    Public Property mPrefijo() As String
        Get
            Return hid_Prefijo.Value
        End Get
        Set(ByVal value As String)
            hid_Prefijo.Value = value
        End Set
    End Property

    Public ReadOnly Property Grid_Correos() As GridView
        Get
            Return gvd_Correos
        End Get
    End Property

    Public ReadOnly Property Grid_Autorizacion() As GridView
        Get
            Return gvd_Autorizacion
        End Get
    End Property

    Public Property Titulo_Autoriza() As String
        Get
            Return lbl_TituloAutoriza.Text
        End Get
        Set(ByVal value As String)
            lbl_TituloAutoriza.Text = value
        End Set
    End Property


    Public Property cod_modulo() As Integer
        Get
            Return hid_modulo.Value
        End Get
        Set(ByVal value As Integer)
            hid_modulo.Value = value
        End Set
    End Property

    Public Property cod_submodulo() As Integer
        Get
            Return hid_submodulo.Value
        End Get
        Set(ByVal value As Integer)
            hid_submodulo.Value = value
        End Set
    End Property

    Public Property Alta() As Integer
        Get
            Return hid_Alta.Value
        End Get
        Set(ByVal value As Integer)
            hid_Alta.Value = value
        End Set
    End Property

    Public Property Baja() As Integer
        Get
            Return hid_Baja.Value
        End Get
        Set(ByVal value As Integer)
            hid_Baja.Value = value
        End Set
    End Property

    Public Property Cambio() As Integer
        Get
            Return hid_Cambio.Value
        End Get
        Set(ByVal value As Integer)
            hid_Cambio.Value = value
        End Set
    End Property

    Public Property Consulta() As Integer
        Get
            Return hid_Consulta.Value
        End Get
        Set(ByVal value As Integer)
            hid_Consulta.Value = value
        End Set
    End Property

    Private Sub Pages_SiteMaster_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            menu_principal.InnerHtml = Session("Menu")
            If Not IsPostBack Then

                'Recupera la Información General
                InformacionGeneral()

                'Evalua los permisos del aplicativo
                'EvaluaPermisosModulo()


                url_reportes = Funciones.fn_Parametro(Cons.ReportesProduccion)
                Funciones.LlenaCatDDL(ddl_SucursalPol, "Suc",,,,,, False)
                Funciones.LlenaCatDDL(ddl_ConceptoAnula, "Anu",,,,,, False)
                CargaRowOculto()
            End If


        Catch ex As Exception
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "Pages_SiteMaster_Load: " & ex.Message)
        End Try

    End Sub

    Public Sub InformacionGeneral()
        DetalleUsuario = Split(Context.User.Identity.Name, "|")
        If DetalleUsuario.Count > 1 Then
            cod_usuario = DetalleUsuario(0)
            lbl_Usuario.Text = DetalleUsuario(1)
            hid_codSuc.Value = DetalleUsuario(2)
            lbl_Sucursal.Text = DetalleUsuario(3)
            hid_codSector.Value = DetalleUsuario(4)
            lbl_Sector.Text = DetalleUsuario(5)
            hid_mail.Value = DetalleUsuario(6)
            hid_user.Value = DetalleUsuario(7)
            hid_pass.Value = DetalleUsuario(8)
        End If
    End Sub

    Private Sub CargaRowOculto()
        Dim dummy As New DataTable()
        dummy.Columns.Add("Clave")
        dummy.Columns.Add("Descripcion")
        dummy.Columns.Add("OcultaCampo1")
        dummy.Rows.Add()
        gvd_Catalogo.DataSource = dummy
        gvd_Catalogo.DataBind()
    End Sub

    'Evento que evalua los elementos seleccionados en el Catalogo
    Private Sub btn_Aceptar_Catalogo_Click(sender As Object, e As EventArgs) Handles btn_Aceptar_Catalogo.Click
        Try
            Dim Datos() As String
            Dim Seleccionados As String = hid_Seleccion.Value
            Dim lbl_Oculta1 As Object
            Dim lbl_Oculta2 As Object
            Dim lbl_Oculta3 As Object
            Dim lbl_Oculta4 As Object
            Dim lbl_Oculta5 As Object
            Dim lbl_Oculta6 As Object

            Dim Elemento As String
            Dim OcultaCampo4 As String = vbNullString
            Dim OcultaCampo5 As String = vbNullString
            Dim OcultaCampo6 As String = vbNullString



            If Len(Seleccionados) > 0 Then

                Dim gvd_Control As GridView = TryCast(cph_principal.FindControl(hid_Control.Value), GridView)

                Dim Prefijo As String = hid_Prefijo.Value
                Datos = Split(Seleccionados.Substring(0, Seleccionados.Length - 1), "|")

                If Not gvd_Control Is Nothing Then

                    dt_Datos = New DataTable
                    dt_Datos.Columns.Add("Clave")
                    dt_Datos.Columns.Add("Descripcion")

                    dt_Datos.Columns.Add("OcultaCampo1")
                    dt_Datos.Columns.Add("OcultaCampo2")
                    dt_Datos.Columns.Add("OcultaCampo3")

                    For Each row As GridViewRow In gvd_Control.Rows


                        If TryCast(row.FindControl("chk_Sel" & Prefijo), HiddenField) Is Nothing Then
                            Elemento = IIf(DirectCast(row.FindControl("chk_Sel" & Prefijo), CheckBox).Checked, "true", "false")
                        Else
                            Elemento = DirectCast(row.FindControl("chk_Sel" & Prefijo), HiddenField).Value
                        End If


                        If Elemento <> "true" Then
                            Dim Fila As DataRow = dt_Datos.NewRow()
                            Fila("Clave") = DirectCast(row.FindControl("lbl_Clave" & Prefijo), Label).Text
                            Fila("Descripcion") = DirectCast(row.FindControl("lbl_Desc"), Label).Text
                            Fila("OcultaCampo1") = ""
                            Fila("OcultaCampo2") = ""
                            Fila("OcultaCampo3") = ""


                            'Controles adicionles, no vienen de base de datos, concatenar 3 en cada control Oculta fijo
                            lbl_Oculta4 = row.FindControl("lbl_Oculta4")
                            lbl_Oculta5 = row.FindControl("lbl_Oculta5")
                            lbl_Oculta6 = row.FindControl("lbl_Oculta6")



                            If Not lbl_Oculta4 Is Nothing Then
                                Select Case lbl_Oculta4.GetType()
                                    Case GetType(System.Web.UI.WebControls.Label), GetType(System.Web.UI.WebControls.TextBox)
                                        OcultaCampo4 = "|" & lbl_Oculta4.Text
                                    Case GetType(System.Web.UI.WebControls.CheckBox)
                                        OcultaCampo4 = "|" & lbl_Oculta4.Checked
                                End Select
                            End If

                            If Not lbl_Oculta5 Is Nothing Then
                                Select Case lbl_Oculta5.GetType()
                                    Case GetType(System.Web.UI.WebControls.Label), GetType(System.Web.UI.WebControls.TextBox)
                                        OcultaCampo5 = "|" & lbl_Oculta5.Text
                                    Case GetType(System.Web.UI.WebControls.CheckBox)
                                        OcultaCampo5 = "|" & lbl_Oculta5.Checked
                                End Select
                            End If

                            If Not lbl_Oculta6 Is Nothing Then
                                Select Case lbl_Oculta6.GetType()
                                    Case GetType(System.Web.UI.WebControls.Label), GetType(System.Web.UI.WebControls.TextBox)
                                        OcultaCampo6 = "|" & lbl_Oculta6.Text
                                    Case GetType(System.Web.UI.WebControls.CheckBox)
                                        OcultaCampo6 = "|" & lbl_Oculta6.Checked
                                End Select
                            End If

                            'Controles Pirncipales y fijos
                            lbl_Oculta1 = row.FindControl("lbl_Oculta1")
                            lbl_Oculta2 = row.FindControl("lbl_Oculta2")
                            lbl_Oculta3 = row.FindControl("lbl_Oculta3")

                            If Not lbl_Oculta1 Is Nothing Then
                                Select Case lbl_Oculta1.GetType()
                                    Case GetType(System.Web.UI.WebControls.Label), GetType(System.Web.UI.WebControls.TextBox)
                                        Fila("OcultaCampo1") = lbl_Oculta1.Text & OcultaCampo4 & OcultaCampo5 & OcultaCampo6
                                    Case GetType(System.Web.UI.WebControls.DropDownList)
                                        Fila("OcultaCampo1") = lbl_Oculta1.SelectedValue & OcultaCampo4 & OcultaCampo5 & OcultaCampo6
                                    Case GetType(System.Web.UI.WebControls.HiddenField)
                                        Fila("OcultaCampo1") = lbl_Oculta1.Value & OcultaCampo4 & OcultaCampo5 & OcultaCampo6
                                    Case GetType(System.Web.UI.WebControls.CheckBox)
                                        Fila("OcultaCampo1") = lbl_Oculta1.Checked & OcultaCampo4 & OcultaCampo5 & OcultaCampo6
                                End Select
                            End If

                            If Not lbl_Oculta2 Is Nothing Then
                                Select Case lbl_Oculta2.GetType()
                                    Case GetType(System.Web.UI.WebControls.Label), GetType(System.Web.UI.WebControls.TextBox)
                                        Fila("OcultaCampo2") = lbl_Oculta2.Text
                                    Case GetType(System.Web.UI.WebControls.DropDownList)
                                        Fila("OcultaCampo2") = lbl_Oculta2.SelectedValue
                                    Case GetType(System.Web.UI.WebControls.HiddenField)
                                        Fila("OcultaCampo2") = lbl_Oculta2.Value
                                    Case GetType(System.Web.UI.WebControls.CheckBox)
                                        Fila("OcultaCampo2") = lbl_Oculta2.Checked
                                End Select
                            End If

                            If Not lbl_Oculta3 Is Nothing Then
                                Select Case lbl_Oculta3.GetType()
                                    Case GetType(System.Web.UI.WebControls.Label), GetType(System.Web.UI.WebControls.TextBox)
                                        Fila("OcultaCampo3") = lbl_Oculta3.Text
                                    Case GetType(System.Web.UI.WebControls.DropDownList)
                                        Fila("OcultaCampo3") = lbl_Oculta3.SelectedValue
                                    Case GetType(System.Web.UI.WebControls.HiddenField)
                                        Fila("OcultaCampo3") = lbl_Oculta3.Value
                                    Case GetType(System.Web.UI.WebControls.CheckBox)
                                        Fila("OcultaCampo3") = lbl_Oculta3.Checked
                                End Select
                            End If

                            dt_Datos.Rows.Add(Fila)
                        End If
                    Next

                    For Each dato In Datos
                        Dim Fila As DataRow = dt_Datos.NewRow()
                        Fila("Clave") = Split(dato, "~")(0)
                        Fila("Descripcion") = Split(dato, "~")(1)
                        Fila("OcultaCampo1") = Split(dato, "~")(2)
                        Fila("OcultaCampo2") = Split(dato, "~")(3)
                        Fila("OcultaCampo3") = Split(dato, "~")(4)
                        dt_Datos.Rows.Add(Fila)
                    Next

                    gvd_Control.DataSource = dt_Datos
                    gvd_Control.DataBind()
                Else
                    If Len(hid_Control.Value) > 0 Then
                        Dim Controles() As String = Split(hid_Control.Value, "|")
                        Dim subindice As Integer

                        Select Case Controles.Count
                            Case 1
                                If Left(Controles(0), 3) = "lst" Then
                                    Dim lst_Clave As ListBox = DirectCast(cph_principal.FindControl(Controles(0)), ListBox)
                                    If Not lst_Clave Is Nothing Then
                                        Datos = Split(Seleccionados.Substring(0, Seleccionados.Length - 1), "|")
                                        For Each dato In Datos
                                            subindice = 0
                                            For Each item In lst_Clave.Items
                                                If Split(dato, "~")(0) = Split(item.value, "-")(0) Then
                                                    subindice = subindice + 1
                                                End If
                                            Next

                                            lst_Clave.Items.Add(New ListItem(Split(dato, "~")(0) & ".-" & Split(dato, "~")(1), Split(dato, "~")(0) & "-" & subindice))
                                        Next
                                    End If
                                Else
                                    Dim txt_Clave As TextBox = DirectCast(cph_principal.FindControl(Controles(0)), TextBox)
                                    If txt_Clave Is Nothing Then
                                        txt_Clave = DirectCast(frmMaster.FindControl(Controles(0)), TextBox)
                                    End If
                                    txt_Clave.Text = Split(Datos(0), "~")(0)
                                End If
                            Case 2
                                Dim txt_Clave As TextBox = DirectCast(cph_principal.FindControl(Controles(0)), TextBox)
                                Dim txt_Descripcion As TextBox = DirectCast(cph_principal.FindControl(Controles(1)), TextBox)
                                Dim txt_RFC As TextBox = DirectCast(cph_principal.FindControl(Controles(2)), TextBox)

                                If txt_Clave Is Nothing And txt_Descripcion Is Nothing Then
                                    txt_Clave = DirectCast(frmMaster.FindControl(Controles(0)), TextBox)
                                    txt_Descripcion = DirectCast(frmMaster.FindControl(Controles(1)), TextBox)
                                End If

                                txt_Clave.Text = Split(Datos(0), "~")(0)
                                txt_Descripcion.Text = Split(Datos(0), "~")(1)
                                txt_RFC.Text = Split(Datos(0), "~")(2)


                            Case 3
                                If Left(Controles(0), 3) = "gvd" Then
                                    Dim gvd_Ctrl As GridView = DirectCast(cph_principal.FindControl(Controles(0)), GridView)
                                    Dim ItemCtrl = DirectCast(gvd_Ctrl.Rows(Controles(1)).FindControl(Controles(2)), Label)
                                    Dim Clave As String = "(" & Split(Datos(0), "~")(0) & ") "
                                    Dim Descrip As String = Split(Datos(0), "~")(1)
                                    ItemCtrl.Text = Clave & Descrip
                                Else
                                    Dim txt_Clave As TextBox = DirectCast(cph_principal.FindControl(Controles(0)), TextBox)
                                    Dim txt_Descripcion As TextBox = DirectCast(cph_principal.FindControl(Controles(1)), TextBox)
                                    Dim txt_RFC As TextBox = DirectCast(cph_principal.FindControl(Controles(2)), TextBox)
                                    'Dim gvd_GridView As GridView = DirectCast(cph_principal.FindControl(Controles(2)), GridView)

                                    'If txt_Clave Is Nothing And txt_Descripcion Is Nothing And gvd_GridView Is Nothing Then
                                    '    txt_Clave = DirectCast(frmMaster.FindControl(Controles(0)), TextBox)
                                    '    txt_Descripcion = DirectCast(frmMaster.FindControl(Controles(1)), TextBox)
                                    '    'gvd_GridView = DirectCast(frmMaster.FindControl(Controles(2)), GridView)
                                    'End If

                                    txt_Clave.Text = Split(Datos(0), "~")(0)
                                    txt_Descripcion.Text = Split(Datos(0), "~")(1)
                                    txt_RFC.Text = Split(Datos(0), "~")(2)
                                    'gvd_GridView.DataSource = Nothing
                                    'gvd_GridView.DataBind()

                                End If
                            Case 4
                                'VZAVALETA_10290_CC_INICIO
                                Dim txt_Clave As TextBox = DirectCast(cph_principal.FindControl(Controles(0)), TextBox)

                                txt_Clave.Text = Split(Datos(0), "~")(0)
                                'VZAVALETA_10290_CC_FIN
                        End Select
                    End If

                End If

            Else
                Mensaje.MuestraMensaje("Catálogo", "No seleccionó ningún elemento", TipoMsg.Advertencia)
            End If

            Funciones.CerrarModal("#Catalogo")
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Aceptar_Catalogo_Click: " & ex.Message)
        End Try
    End Sub

    Public Sub CancelaPolizas()
        Funciones.LlenaGrid(gvd_GrupoPolizas, Nothing)
        dtPolizas = Nothing
        ddl_SucursalPol.Enabled = True
        txtClaveRam.Enabled = True
        txt_NoPoliza.Enabled = True
        btn_Busca_Endoso.Visible = True
        btn_Cancela_Endoso.Visible = False
    End Sub

    Private Sub btn_Busca_Endoso_Click(sender As Object, e As EventArgs) Handles btn_Busca_Endoso.Click
        Try
            Dim Polizas As String = "'-1'"
            Dim ws As New ws_Generales.GeneralesClient
            Dim Elemento As String

            If txtClaveRam.Text = "" Or txtSearchRam.Text = "" Then
                Mensaje.MuestraMensaje(Titulo, "Se debe especificar un Ramo de Póliza", TipoMsg.Falla)
                Exit Sub
            End If

            If txt_NoPoliza.Text = "" Then
                Mensaje.MuestraMensaje(Titulo, "Se debe especificar un Número de Póliza", TipoMsg.Falla)
                Exit Sub
            End If

            Dim gvd_Control As GridView = DirectCast(cph_principal.FindControl(hid_Control_Pol.Value), GridView)
            If Not gvd_Control Is Nothing Then
                For Each row In gvd_Control.Rows

                    If TryCast(row.FindControl("chk_SelPol"), HiddenField) Is Nothing Then
                        Elemento = IIf(DirectCast(row.FindControl("chk_SelPol"), CheckBox).Checked, "true", "false")
                    Else
                        Elemento = DirectCast(row.FindControl("chk_SelPol"), HiddenField).Value
                    End If

                    If Elemento <> "true" Then
                        Polizas = Polizas & ",'" & DirectCast(row.FindControl("lbl_ClavePol"), Label).Text & "'"
                    End If
                Next
            End If
            Dim SubModWeb As Integer = Session("SubModWeb")

            dtPolizas = Funciones.Lista_A_Datatable(ws.ObtienePolizas(ddl_SucursalPol.SelectedValue, txtClaveRam.Text,
                                                                      txt_NoPoliza.Text, Polizas, False, "", "",
                                                                      IIf(gvd_GrupoPolizas.Columns(6).Visible = True, 1, 0), SubModWeb).ToList)


            'Si se filtra por sufijo
            If hid_sufijo.Value > 0 Then
                Dim myRow() As Data.DataRow
                myRow = dtPolizas.Select("aaaa_endoso = " & hid_sufijo.Value)
                dtPolizas = myRow.CopyToDataTable
            End If


            If dtPolizas.Rows.Count > 0 Then
                gvd_GrupoPolizas.PageIndex = 0
                Funciones.LlenaGrid(gvd_GrupoPolizas, dtPolizas)
                ddl_SucursalPol.Enabled = False
                txtClaveRam.Enabled = False
                txt_NoPoliza.Enabled = False
                btn_Busca_Endoso.Visible = False
                btn_Cancela_Endoso.Visible = True

                If Cambio = 0 Then
                    For Each row In gvd_GrupoPolizas.Rows
                        TryCast(row.FindControl("chk_NoPago"), CheckBox).Enabled = False
                    Next
                End If

            Else
                Funciones.LlenaGrid(gvd_GrupoPolizas, Nothing)
                btn_Cancela_Endoso_Click(Me, Nothing)
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Busca_Endoso_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Cancela_Endoso_Click(sender As Object, e As EventArgs) Handles btn_Cancela_Endoso.Click
        Try
            CancelaPolizas()
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Cancela_Endoso_Click: " & ex.Message)
        End Try
    End Sub

    Public Sub MuestraPolizario(ByVal Control As String, Optional ByVal sn_garantias As Boolean = True,
                                Optional ByVal sn_ajuste As Boolean = True, Optional ByVal sn_aclaraciones As Boolean = True,
                                Optional ByVal sn_cobranzas As Boolean = True, Optional ByVal sn_descarta_endoso As Boolean = True,
                                Optional ByVal sn_descarta_ac As Boolean = True, Optional ByVal sn_submod_web As Integer = -1,
                                Optional ByVal sucursal As Integer = 0, Optional ByVal cod_ramo As Integer = 0,
                                Optional ByVal nro_pol As Integer = 0, Optional ByVal Sufijo As Integer = 0)

        hid_sufijo.Value = Sufijo

        If sucursal > 0 Then
            ddl_SucursalPol.SelectedValue = sucursal
        End If
        If cod_ramo > 0 Then
            txtClaveRam.Text = cod_ramo
            Dim ws As New ws_Generales.GeneralesClient
            Dim dtConsulta As New DataTable

            dtConsulta = Funciones.Lista_A_Datatable(ws.ObtieneCatalogo("Pro", "AND cod_ramo=" & cod_ramo, "").ToList)
            If Not dtConsulta Is Nothing Then
                txtSearchRam.Text = dtConsulta.Rows(0).ItemArray(2).ToString
            End If
        End If


        If nro_pol > 0 Then
            txt_NoPoliza.Text = nro_pol
        End If

        If hid_sufijo.Value > 0 Then
            btn_Busca_Endoso.Visible = False
            btn_Aceptar_Endoso.Visible = False
        Else
            btn_Busca_Endoso.Visible = True
            btn_Aceptar_Endoso.Visible = True
        End If

        hid_Control_Pol.Value = Control

        gvd_GrupoPolizas.Columns(6).Visible = sn_ajuste
        gvd_GrupoPolizas.Columns(7).Visible = sn_aclaraciones
        gvd_GrupoPolizas.Columns(8).Visible = sn_cobranzas
        gvd_GrupoPolizas.Columns(9).Visible = sn_descarta_endoso
        gvd_GrupoPolizas.Columns(10).Visible = sn_descarta_ac

        If sn_descarta_ac = True Then
            Session.Add("SubModWeb", "")
            Session("SubModWeb") = sn_submod_web
        End If


        If btn_Busca_Endoso.Visible = False Then
            btn_Busca_Endoso_Click(Me, Nothing)
        End If

        Funciones.AbrirModal("#Poliza")
    End Sub

    Private Sub btn_Aceptar_Endoso_Click(sender As Object, e As EventArgs) Handles btn_Aceptar_Endoso.Click
        Try
            Dim Elemento As String

            dt_Datos = New DataTable
            dt_Datos.Columns.Add("Clave")
            dt_Datos.Columns.Add("Descripcion")
            dt_Datos.Columns.Add("id_pv")


            Dim gvd_Control As GridView = DirectCast(cph_principal.FindControl(hid_Control_Pol.Value), GridView)

            For Each row As GridViewRow In gvd_Control.Rows

                If TryCast(row.FindControl("chk_SelPol"), HiddenField) Is Nothing Then
                    Elemento = IIf(DirectCast(row.FindControl("chk_SelPol"), CheckBox).Checked, "true", "false")
                Else
                    Elemento = DirectCast(row.FindControl("chk_SelPol"), HiddenField).Value
                End If

                If Elemento <> "true" Then
                    Dim Fila As DataRow = dt_Datos.NewRow()
                    Fila("Clave") = DirectCast(row.FindControl("lbl_ClavePol"), Label).Text
                    Fila("Descripcion") = DirectCast(row.FindControl("lbl_DescripcionPol"), Label).Text
                    Fila("id_pv") = DirectCast(row.FindControl("lbl_idpv"), Label).Text
                    dt_Datos.Rows.Add(Fila)
                End If
            Next

            For Each row As GridViewRow In gvd_GrupoPolizas.Rows
                Dim NewRow As DataRow = dt_Datos.NewRow()
                Dim chk_SelPol As CheckBox = DirectCast(row.FindControl("chk_SelPol"), CheckBox)

                If chk_SelPol.Checked = True Then
                    Dim vId_pv = gvd_GrupoPolizas.DataKeys(row.RowIndex)("id_pv")
                    Dim txt_Sucursal As Label = DirectCast(row.FindControl("txt_Sucursal"), Label)
                    Dim txt_Ramo As Label = DirectCast(row.FindControl("txt_Ramo"), Label)
                    Dim txt_Poliza As Label = DirectCast(row.FindControl("txt_Poliza"), Label)
                    Dim txt_Sufijo As Label = DirectCast(row.FindControl("txt_Sufijo"), Label)
                    Dim txt_Endoso As Label = DirectCast(row.FindControl("txt_Endoso"), Label)
                    Dim txt_Ajuste As Label = DirectCast(row.FindControl("txt_Ajuste"), Label)
                    Dim lbl_GrupoEndoso As Label = DirectCast(row.FindControl("lbl_GrupoEndoso"), Label)

                    NewRow("Clave") = txt_Sucursal.Text & "-" & txt_Ramo.Text & "-" & txt_Poliza.Text & "-" &
                                      txt_Sufijo.Text & "-" & txt_Endoso.Text &
                                      IIf(gvd_GrupoPolizas.Columns(6).Visible = True, " Aj:" & txt_Ajuste.Text, "")
                    NewRow("Descripcion") = lbl_GrupoEndoso.Text
                    NewRow("id_pv") = vId_pv
                    dt_Datos.Rows.Add(NewRow)
                End If
            Next

            gvd_Control.DataSource = dt_Datos
            gvd_Control.DataBind()

            btn_Busca_Endoso_Click(Me, Nothing)

        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Aceptar_Endoso_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub gvd_GrupoPolizas_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvd_GrupoPolizas.RowDataBound
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim cod_grupo_endo As Integer = sender.DataKeys(e.Row.RowIndex)("cod_grupo_endo")

                Dim txt_Sufijo As Label = TryCast(e.Row.FindControl("txt_Sufijo"), Label)
                Dim txt_Endoso As Label = TryCast(e.Row.FindControl("txt_Endoso"), Label)
                Dim lbl_Emision As Label = TryCast(e.Row.FindControl("lbl_Emision"), Label)
                Dim lbl_Desde As Label = TryCast(e.Row.FindControl("lbl_Desde"), Label)
                Dim lbl_Hasta As Label = TryCast(e.Row.FindControl("lbl_Hasta"), Label)
                Dim lbl_GrupoEndoso As Label = TryCast(e.Row.FindControl("lbl_GrupoEndoso"), Label)
                Dim lbl_GrupoTipoEndoso As Label = TryCast(e.Row.FindControl("lbl_GrupoTipoEndoso"), Label)
                Dim lbl_PrimaEmitida As Label = TryCast(e.Row.FindControl("lbl_PrimaEmitida"), Label)
                Dim lbl_PrimaAplicada As Label = TryCast(e.Row.FindControl("lbl_PrimaAplicada"), Label)
                Dim lbl_PrimaPagada As Label = TryCast(e.Row.FindControl("lbl_PrimaPagada"), Label)
                Dim lbl_Asegurado As Label = TryCast(e.Row.FindControl("lbl_Asegurado"), Label)
                Dim lbl_Emisor As Label = TryCast(e.Row.FindControl("lbl_Emisor"), Label)
                Dim lbl_Reasegurador As Label = TryCast(e.Row.FindControl("lbl_Reasegurador"), Label)
                Dim lbl_Suscriptor As Label = TryCast(e.Row.FindControl("lbl_Suscriptor"), Label)
                Dim div_Ajuste As HtmlGenericControl = TryCast(e.Row.FindControl("div_Ajuste"), HtmlGenericControl)
                Dim txt_Ajuste As Label = TryCast(e.Row.FindControl("txt_Ajuste"), Label)
                Dim lbl_Liberacion As Label = TryCast(e.Row.FindControl("lbl_Liberacion"), Label)

                If cod_grupo_endo = 3 Or cod_grupo_endo = 15 Or cod_grupo_endo = 19 Then
                    txt_Sufijo.BackColor = Drawing.Color.Orange
                    txt_Endoso.BackColor = Drawing.Color.Orange
                    lbl_Emision.BackColor = Drawing.Color.Orange
                    lbl_Desde.BackColor = Drawing.Color.Orange
                    lbl_Hasta.BackColor = Drawing.Color.Orange
                    lbl_GrupoEndoso.BackColor = Drawing.Color.Orange
                    lbl_GrupoTipoEndoso.BackColor = Drawing.Color.Orange
                    lbl_PrimaEmitida.BackColor = Drawing.Color.Orange
                    lbl_PrimaAplicada.BackColor = Drawing.Color.Orange
                    lbl_PrimaPagada.BackColor = Drawing.Color.Orange
                    lbl_Asegurado.BackColor = Drawing.Color.Orange
                    lbl_Emisor.BackColor = Drawing.Color.Orange
                    lbl_Reasegurador.BackColor = Drawing.Color.Orange
                    lbl_Suscriptor.BackColor = Drawing.Color.Orange
                    txt_Ajuste.BackColor = Drawing.Color.Orange
                    lbl_Liberacion.BackColor = Drawing.Color.Orange
                ElseIf cod_grupo_endo = 1 Or cod_grupo_endo = 4 Then
                    txt_Sufijo.BackColor = Drawing.Color.LightGreen
                    txt_Endoso.BackColor = Drawing.Color.LightGreen
                    lbl_Emision.BackColor = Drawing.Color.LightGreen
                    lbl_Desde.BackColor = Drawing.Color.LightGreen
                    lbl_Hasta.BackColor = Drawing.Color.LightGreen
                    lbl_GrupoEndoso.BackColor = Drawing.Color.LightGreen
                    lbl_GrupoTipoEndoso.BackColor = Drawing.Color.LightGreen
                    lbl_PrimaEmitida.BackColor = Drawing.Color.LightGreen
                    lbl_PrimaAplicada.BackColor = Drawing.Color.LightGreen
                    lbl_PrimaPagada.BackColor = Drawing.Color.LightGreen
                    lbl_Asegurado.BackColor = Drawing.Color.LightGreen
                    lbl_Emisor.BackColor = Drawing.Color.LightGreen
                    lbl_Reasegurador.BackColor = Drawing.Color.LightGreen
                    lbl_Suscriptor.BackColor = Drawing.Color.LightGreen
                    txt_Ajuste.BackColor = Drawing.Color.LightGreen
                    lbl_Liberacion.BackColor = Drawing.Color.LightGreen
                End If
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "gvd_GrupoPolizas_RowDataBound: " & ex.Message)
        End Try
    End Sub

    Private Sub gvd_GrupoPolizas_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvd_GrupoPolizas.PageIndexChanging
        Try
            Dim myRow() As Data.DataRow

            For Each Row In gvd_GrupoPolizas.Rows
                Dim chk_SelPol As CheckBox = DirectCast(Row.FindControl("chk_SelPol"), CheckBox)
                Dim chk_NoPago As CheckBox = DirectCast(Row.FindControl("chk_NoPago"), CheckBox)
                Dim chk_NoAC As CheckBox = DirectCast(Row.FindControl("chk_NoAC"), CheckBox)
                Dim id_pv As Integer = gvd_GrupoPolizas.DataKeys(Row.RowIndex)("id_pv")

                myRow = dtPolizas.Select("id_pv ='" & id_pv & "'")
                myRow(0)("tSEL_Val") = chk_SelPol.Checked
                myRow(0)("sn_NoPago") = chk_NoPago.Checked
                myRow(0)("sn_NoAC") = chk_NoAC.Checked
            Next

            gvd_GrupoPolizas.PageIndex = e.NewPageIndex
            Funciones.LlenaGrid(gvd_GrupoPolizas, dtPolizas)

        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "gvd_GrupoPolizas_PageIndexChanging: " & ex.Message)
        End Try
    End Sub

    Private Sub gvd_GrupoPolizas_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvd_GrupoPolizas.RowCommand
        Try
            If e.CommandName.Equals("Cobranzas") Then
                Dim Index As Integer = e.CommandSource.NamingContainer.RowIndex
                Dim id_pv As Integer = gvd_GrupoPolizas.DataKeys(Index)("id_pv")

                Dim cod_suc As Integer = gvd_GrupoPolizas.DataKeys(Index)("cod_suc")
                Dim cod_ramo As Integer = gvd_GrupoPolizas.DataKeys(Index)("cod_ramo")
                Dim nro_pol As Integer = gvd_GrupoPolizas.DataKeys(Index)("nro_pol")
                Dim aaaa_endoso As Integer = gvd_GrupoPolizas.DataKeys(Index)("aaaa_endoso")
                Dim nro_endoso As Integer = gvd_GrupoPolizas.DataKeys(Index)("nro_endoso")

                DetalleCobranzas(id_pv, {cod_suc & "-" & cod_ramo & "-" & nro_pol & "-" & aaaa_endoso}, nro_endoso)

                Funciones.AbrirModal("#CobranzasModal")
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "gvd_GrupoPolizas_RowCommand: " & ex.Message)
        End Try
    End Sub

    Private Sub SeleccionElementoGread(ByRef gvd_Control As GridView, ByVal id_elemento As Integer)
        For Each row In gvd_Control.Rows
            TryCast(row.FindControl("lnk_Elemento"), LinkButton).BackColor = Drawing.Color.White
            If TryCast(row.FindControl("lnk_Elemento"), LinkButton).Text = id_elemento Then
                TryCast(row.FindControl("lnk_Elemento"), LinkButton).BackColor = Drawing.Color.Yellow
                TryCast(row.FindControl("lnk_Elemento"), LinkButton).Focus()
            End If
        Next
    End Sub

    Public Sub DetalleCobranzas(ByVal id_pv As Integer, ByVal Poliza() As String, ByVal nro_endoso As Integer, Optional ByVal Asegurado As String = vbNullString)

        hid_Poliza.Value = Poliza(0)

        ddl_PolizaCobranzas.Items.Clear()

        For Each item In Poliza
            ddl_PolizaCobranzas.Items.Add(item)
        Next

        lbl_AseguradoCobranzas.Text = Asegurado

        Funciones.fn_Consulta("spS_ListaEndoso '''" & Poliza(0) & "''',''", dt_Consulta)
        Funciones.LlenaGrid(gvd_Endosos, dt_Consulta)

        'Evalua el cambio en Cobranzas
        If Cambio = 0 Then
            For Each row In gvd_Endosos.Rows
                TryCast(row.FindControl("chk_Financiado"), CheckBox).Enabled = False
                TryCast(row.FindControl("chk_Pago"), CheckBox).Enabled = False
                TryCast(row.FindControl("chk_Recargo"), CheckBox).Enabled = False
            Next
        End If

        If id_pv = 0 Then
            id_pv = gvd_Endosos.DataKeys(0)("id_pv")
            nro_endoso = gvd_Endosos.DataKeys(0)("nro_endoso")
        End If

        ConsultaPagadores(id_pv, nro_endoso)
        SeleccionElementoGread(gvd_Endosos, nro_endoso)

        Funciones.EjecutaFuncion("fn_Resizable('#Endosos');", "Tamaño")
    End Sub

    Private Sub ConsultaPagadores(ByVal id_pv As Integer, ByVal nro_endoso As Integer)
        Dim ws As New ws_Generales.GeneralesClient

        'PAGADORES
        lbl_Pagadores.Text = "PAGADORES ENDOSO >> " & nro_endoso
        Funciones.LlenaGrid(gvd_Pagadores, Funciones.Lista_A_Datatable(ws.ObtienePagador(id_pv).ToList))

        If gvd_Pagadores.Rows.Count > 0 Then
            TryCast(gvd_Pagadores.Rows(0).FindControl("lnk_Elemento"), LinkButton).BackColor = Drawing.Color.Yellow

            'CUOTAS
            ConsultaCuotas(id_pv, gvd_Pagadores.DataKeys(0)("cod_aseg"), gvd_Pagadores.DataKeys(0)("cod_ind_pagador"))

            If gvd_PagadorCuota.Rows.Count > 0 Then
                'DETALLE CUOTAS
                ConsultaDetalleCuotas(id_pv, gvd_PagadorCuota.DataKeys(0)("cod_aseg"), gvd_PagadorCuota.DataKeys(0)("cod_ind_pagador"), gvd_PagadorCuota.DataKeys(0)("nro_cuota"))
            End If
        End If
    End Sub

    Private Sub ConsultaCuotas(ByVal id_pv As Integer, ByVal cod_aseg As Integer, ByVal ind_pagador As Integer)
        Dim ws As New ws_Generales.GeneralesClient

        'CUOTAS
        lbl_DetPagador.Text = "CUOTAS PAGADOR >> " & cod_aseg
        Funciones.LlenaGrid(gvd_PagadorCuota, Funciones.Lista_A_Datatable(ws.ObtienePagadorCuotas(id_pv, ind_pagador, cod_aseg).ToList))

        If gvd_PagadorCuota.Rows.Count > 0 Then
            TryCast(gvd_PagadorCuota.Rows(0).FindControl("lnk_Elemento"), LinkButton).BackColor = Drawing.Color.Yellow

            'DETALLE CUOTAS
            ConsultaDetalleCuotas(id_pv, cod_aseg, ind_pagador, gvd_PagadorCuota.DataKeys(0)("nro_cuota"))
        End If
    End Sub

    Private Sub ConsultaDetalleCuotas(ByVal id_pv As Integer, ByVal cod_aseg As Integer, ByVal ind_pagador As Integer, ByVal nro_cuota As Integer)
        Dim ws As New ws_Generales.GeneralesClient

        'DETALLE CUOTAS
        lbl_DetCuota.Text = "PAGOS CUOTA >> " & nro_cuota
        Funciones.LlenaGrid(gvd_DetCuotaPagador, Funciones.Lista_A_Datatable(ws.ObtieneDetallePagoCuota(id_pv, cod_aseg, ind_pagador, nro_cuota).ToList))
    End Sub

    Private Sub gvd_Endosos_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvd_Endosos.RowCommand
        Try
            If e.CommandName.Equals("DetalleEndoso") Then
                Dim Index As Integer = e.CommandSource.NamingContainer.RowIndex
                ConsultaPagadores(gvd_Endosos.DataKeys(Index)("id_pv"), gvd_Endosos.DataKeys(Index)("nro_endoso"))

                SeleccionElementoGread(gvd_Endosos, gvd_Endosos.DataKeys(Index)("nro_endoso"))
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "gvd_Endosos_RowCommand: " & ex.Message)
        End Try
    End Sub

    Private Sub gvd_Pagadores_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvd_Pagadores.RowCommand
        Try
            If e.CommandName.Equals("DetallePagador") Then

                Dim Index As Integer = e.CommandSource.NamingContainer.RowIndex

                'CUOTAS
                ConsultaCuotas(gvd_Pagadores.DataKeys(Index)("id_pv"),
                               gvd_Pagadores.DataKeys(Index)("cod_aseg"),
                               gvd_Pagadores.DataKeys(Index)("cod_ind_pagador"))

                SeleccionElementoGread(gvd_Pagadores, gvd_Pagadores.DataKeys(Index)("cod_aseg"))
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "gvd_Pagadores_RowCommand: " & ex.Message)
        End Try
    End Sub

    Private Sub gvd_PagadorCuota_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvd_PagadorCuota.RowCommand
        Try
            If e.CommandName.Equals("DetalleCuotaPagador") Then
                Dim Index As Integer = e.CommandSource.NamingContainer.RowIndex

                'DETALLE CUOTAS
                ConsultaDetalleCuotas(gvd_PagadorCuota.DataKeys(Index)("id_pv"),
                                      gvd_PagadorCuota.DataKeys(Index)("cod_aseg"),
                                      gvd_PagadorCuota.DataKeys(Index)("cod_ind_pagador"),
                                      gvd_PagadorCuota.DataKeys(Index)("nro_cuota"))

                SeleccionElementoGread(gvd_PagadorCuota, gvd_PagadorCuota.DataKeys(Index)("nro_cuota"))
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "gvd_PagadorCuota_RowCommand: " & ex.Message)
        End Try
    End Sub


    Protected Sub chk_NoPago_CheckedChanged(sender As Object, e As EventArgs)
        Try
            Dim ws As New ws_OrdenPago.OrdenPagoClient

            Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, CheckBox).Parent, DataControlFieldCell).Parent, GridViewRow)
            Dim id_pv As Integer = gvd_GrupoPolizas.DataKeys(gr.RowIndex)("id_pv")

            If sender.checked = True Then
                ws.InsertaPolNoPago(id_pv, cod_usuario)
            Else
                ws.EliminaPolNoPago(id_pv)
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "chk_NoPago_CheckedChanged: " & ex.Message)
        End Try
    End Sub

    Private Sub lnk_CerrarSesion_Click(sender As Object, e As EventArgs) Handles lnk_CerrarSesion.Click
        Try
            Dim oParametros As New Dictionary(Of String, Object)
            FormsAuthentication.SignOut()
            Response.Redirect("../Pages/Login.aspx", False)
            oParametros.Add("Accion", 3)
            oParametros.Add("folioOnbase", 999)
            oParametros.Add("cod_usuario", cod_usuario)

            Funciones.ObtenerDatos("usp_bloqueoFolioOnbase_stro", oParametros)

        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "lnk_CerrarSesion_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Autorizar_Click(sender As Object, e As EventArgs) Handles btn_Autorizar.Click
        Try
            Dim ws As New ws_Generales.GeneralesClient
            Dim dtUsuario As New DataTable
            Dim roles() As Integer = {5802, 5803, 5807}
            Dim pin_code As String = vbNullString

            For Each row In gvd_Autorizacion.Rows
                If Funciones.IsAuthenticated("GMX.COM.MX", TryCast(row.FindControl("txt_usuario"), TextBox).Text, TryCast(row.FindControl("txt_contraseña"), TextBox).Text) = False Then
                    Mensaje.MuestraMensaje(Titulo, "La contraseña de " & TryCast(row.FindControl("txt_usuario"), TextBox).Text & " es incorrecta", TipoMsg.Falla)
                    Exit Sub
                Else
                    If roles.Contains(gvd_Autorizacion.DataKeys(row.RowIndex)("cod_rol")) Then
                        pin_code = Eramake.eCryptography.Encrypt(TryCast(row.FindControl("txt_pin"), TextBox).Text)
                        'Funciones.fn_Consulta("spS_UsuarioXRol -1,-1,'" & cod_usuario & "'", dtUsuario)

                        If Funciones.fn_ObtieneUsuarioRol(gvd_Autorizacion.DataKeys(row.RowIndex)("cod_rol"), dtUsuario, 0, cod_usuario).Rows.Count > 0 Then
                            If pin_code <> dtUsuario.Rows(0)("pin_code") Then
                                Mensaje.MuestraMensaje(Titulo, "El pin_code del usuario " & TryCast(row.FindControl("txt_usuario"), TextBox).Text & " es incorrecto", TipoMsg.Falla)
                                Exit Sub
                            End If
                        Else
                            Mensaje.MuestraMensaje(Titulo, "El usuario " & TryCast(row.FindControl("txt_usuario"), TextBox).Text & " no se encuentra autorizado para firmar", TipoMsg.Falla)
                            Exit Sub
                        End If
                    End If
                End If
            Next

            Funciones.EjecutaFuncion("fn_Repuesta_Autoriza();")
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Autorizar_Click: " & ex.Message)
        End Try
    End Sub

    Protected Sub chk_NoAC_CheckedChanged(sender As Object, e As EventArgs)
        Try
            Dim ws As New ws_RecSiniestros.RecSiniestrosClient

            Dim gr As GridViewRow = DirectCast(DirectCast(DirectCast(sender, CheckBox).Parent, DataControlFieldCell).Parent, GridViewRow)
            Dim id_pv As Integer = gvd_GrupoPolizas.DataKeys(gr.RowIndex)("id_pv")

            Dim SubModWeb As Integer = Session("SubModWeb")

            If sender.checked = True Then
                ws.InsertaPolNoAC(id_pv, Me.cod_usuario, SubModWeb)
            Else
                ws.EliminaPolNoAC(id_pv, SubModWeb)
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "chk_NoAC_CheckedChanged: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_ConfirmaMail_Click(sender As Object, e As EventArgs) Handles btn_ConfirmaMail.Click
        Try
            RaiseEvent _btn_ConfirmaMail_Click()
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_ConfirmaMail_Click: " & ex.Message)
        End Try
    End Sub


    Protected Sub ddl_PolizaCobranzas_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            Funciones.fn_Consulta("spS_ListaEndoso '''" & ddl_PolizaCobranzas.SelectedItem.Text & "''',''", dt_Consulta)
            Funciones.LlenaGrid(gvd_Endosos, dt_Consulta)

            ConsultaPagadores(gvd_Endosos.DataKeys(0)("id_pv"), gvd_Endosos.DataKeys(0)("nro_endoso"))
            SeleccionElementoGread(gvd_Endosos, gvd_Endosos.DataKeys(0)("nro_endoso"))

            Funciones.EjecutaFuncion("fn_Resizable('#Endosos');", "Tamaño")
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "ddl_PolizaCobranzas_SelectedIndexChanged: " & ex.Message)
        End Try
    End Sub

    Private Function fn_ConsultaOP(ByVal nro_op As Integer) As Boolean
        Dim ws As New ws_OrdenPago.OrdenPagoClient
        'dtDetalleOP = Funciones.Lista_A_Datatable(ws.ObtieneOrdenPago(nro_op, "", "", "", "", "", "", "", "", -1, "", "", "", 0, "", "").ToList)

        fn_ConsultaOP = False


        Funciones.fn_Consulta("spS_OrdenPago '" & nro_op & "'," &
                                            "''," &
                                            "''," &
                                            "''," &
                                            "''," &
                                            "''," &
                                            "''," &
                                            "''," &
                                            "''," &
                                            -1 & "," &
                                            "''," &
                                            "''," &
                                            "''," &
                                            "0," &
                                            "''," &
                                            "''", dtDetalleOP)

        If dtDetalleOP.Rows.Count > 0 Then
            dtContabilidad = Funciones.Lista_A_Datatable(ws.ObtieneContabilidadOP(nro_op).ToList)
            fn_ConsultaOP = True
        End If
    End Function

    Private Sub LLenaControl()
        With dtDetalleOP
            lbl_Orden.Text = .Rows(0)("nro_op")
            hid_devolucion.Value = .Rows(0)("sn_devolucion")
            txt_Estatus.Text = .Rows(0)("cod_estatus_op") & ".-" & .Rows(0)("estatus")
            txt_FechaEstimada.Text = .Rows(0)("fec_estim_pago")
            txt_FechaPago.Text = .Rows(0)("fec_pago")
            lbl_Transaccion.Text = IIf(IsDBNull(.Rows(0)("nro_recibo")), "", .Rows(0)("nro_recibo"))
            lbl_Compañia.Text = .Rows(0)("txt_cheque_a_nom")
            'lbl_Sucursal.Text = .Rows(0)("SucEmision")
            lbl_MonedaPago.Text = .Rows(0)("Moneda")
            lbl_TipoCambio.Text = .Rows(0)("imp_cambio")
            lbl_MontoPago.Text = String.Format("{0:#,#0.00}", CDbl(.Rows(0)("Monto")))
            lbl_Impuesto.Text = String.Format("{0:#,#0.00}", CDbl(.Rows(0)("Impuesto")))
            lbl_Clave.Text = .Rows(0)("nro_cuenta_transferencia")
            hid_codCuenta.Value = .Rows(0)("id_cuenta_bancaria")
            lbl_Banco.Text = .Rows(0)("Banco")
            hid_codBanco.Value = .Rows(0)("cod_banco_transferencia")

            lbl_UsuAnula.Text = IIf(IsDBNull(.Rows(0)("Baja")), "", .Rows(0)("Baja"))
            lbl_FecAnula.Text = IIf(IsDBNull(.Rows(0)("fec_baja")), "", .Rows(0)("fec_baja"))

            lbl_TextoOP.Text = .Rows(0)("Texto")

            lbl_UsuReaseguro.Text = IIf(IsDBNull(.Rows(0)("Solicitante")), "", .Rows(0)("Solicitante"))
            lbl_FecReaseguro.Text = IIf(IsDBNull(.Rows(0)("fec_generacion")), "", .Rows(0)("fec_generacion"))

            lbl_UsuTesoreria.Text = IIf(IsDBNull(.Rows(0)("Tesoreria")), "", .Rows(0)("Tesoreria"))
            lbl_FecTesoreria.Text = IIf(IsDBNull(.Rows(0)("fec_autoriz_sector")), "", .Rows(0)("fec_autoriz_sector"))

            lbl_UsuContabilidad.Text = IIf(IsDBNull(.Rows(0)("Contabilidad")), "", .Rows(0)("Contabilidad"))
            lbl_FecContabilidad.Text = IIf(IsDBNull(.Rows(0)("fec_autoriz_contab")), "", .Rows(0)("fec_autoriz_contab"))

            Funciones.LlenaGrid(gvd_Contabilidad, dtContabilidad)

            Totales_Contabilidad()
        End With
    End Sub

    Private Sub Totales_Contabilidad()
        Dim dblPrimaBruta As Double = 0
        Dim dblComision As Double = 0
        Dim dblPrimaNeta As Double = 0
        Dim dblISRRet As Double = 0
        Dim dblISRDev As Double = 0

        For Each row In gvd_Contabilidad.Rows
            dblPrimaBruta = dblPrimaBruta + CDbl(TryCast(row.FindControl("hid_Prima"), TextBox).Text)
            dblComision = dblComision + CDbl(TryCast(row.FindControl("hid_Comision"), TextBox).Text)
            dblPrimaNeta = dblPrimaNeta + CDbl(TryCast(row.FindControl("hid_PrimaNeta"), TextBox).Text)
            dblISRRet = dblISRRet + CDbl(TryCast(row.FindControl("hid_MontoISR"), TextBox).Text)
            dblISRDev = dblISRDev + CDbl(TryCast(row.FindControl("hid_MontoISRDev"), TextBox).Text)
        Next

        txt_Prima.Text = String.Format("{0:#,#0.00}", dblPrimaBruta)
        txt_Comision.Text = String.Format("{0:#,#0.00}", dblComision)
        txt_PrimaNeta.Text = String.Format("{0:#,#0.00}", dblPrimaNeta)
        txt_MontoISR.Text = String.Format("{0:#,#0.00}", dblISRRet)
        txt_MontoISRDev.Text = String.Format("{0:#,#0.00}", dblISRDev)
    End Sub

    Private Sub LimpiaControl()
        hid_devolucion.Value = 0
        txt_Estatus.Text = vbNullString
        txt_FechaEstimada.Text = vbNullString
        txt_FechaPago.Text = vbNullString
        lbl_Transaccion.Text = vbNullString
        lbl_Compañia.Text = vbNullString
        'lbl_Sucursal.Text = vbNullString
        lbl_MonedaPago.Text = vbNullString
        lbl_TipoCambio.Text = vbNullString
        lbl_MontoPago.Text = vbNullString
        lbl_Impuesto.Text = vbNullString
        lbl_Clave.Text = vbNullString
        hid_codCuenta.Value = vbNullString
        lbl_Banco.Text = vbNullString
        hid_codBanco.Value = vbNullString

        lbl_UsuAnula.Text = vbNullString
        lbl_FecAnula.Text = vbNullString

        lbl_TextoOP.Text = vbNullString

        lbl_UsuReaseguro.Text = vbNullString
        lbl_FecReaseguro.Text = vbNullString

        lbl_UsuTesoreria.Text = vbNullString
        lbl_FecTesoreria.Text = vbNullString

        lbl_UsuContabilidad.Text = vbNullString
        lbl_FecContabilidad.Text = vbNullString

        dtDetalleOP = Nothing
        dtContabilidad = Nothing
        Funciones.LlenaGrid(gvd_Contabilidad, Nothing)

        lbl_UsuReaseguro.Text = vbNullString
        lbl_FecReaseguro.Text = vbNullString

        lbl_UsuTesoreria.Text = vbNullString
        lbl_FecTesoreria.Text = vbNullString

        lbl_UsuContabilidad.Text = vbNullString
        lbl_FecContabilidad.Text = vbNullString
    End Sub

    Private Sub EdoControl(ByVal intOperacion As Integer)
        Select Case intOperacion
            Case Operacion.Consulta, Operacion.Anula
                lbl_Orden.Enabled = False
                txt_FechaEstimada.Enabled = False
                txt_FechaPago.Enabled = False
                btn_CambiaCuenta.Visible = False
                lbl_EtiqBanco.Visible = True
                lbl_TextoOP.Enabled = False
                EdoCapturaMontos(False)

                btn_Consultar.Visible = False
                btn_Anular.Visible = IIf(intOperacion = Operacion.Consulta And Baja = -1, True, False)
                btn_Guardar.Visible = IIf(intOperacion = Operacion.Consulta, False, True)
                btn_Cancelar.Visible = True

                lbl_Anula.Visible = IIf(intOperacion = Operacion.Anula, True, False)
                ddl_ConceptoAnula.Visible = IIf(intOperacion = Operacion.Anula, True, False)

            Case Operacion.Modifica
                lbl_Orden.Enabled = False
                txt_FechaEstimada.Enabled = True
                txt_FechaPago.Enabled = False
                btn_CambiaCuenta.Visible = True
                lbl_EtiqBanco.Visible = False
                lbl_TextoOP.Enabled = True
                EdoCapturaMontos(True)

                btn_Consultar.Visible = False
                btn_Anular.Visible = False
                btn_Guardar.Visible = True
                btn_Cancelar.Visible = True



            Case Operacion.Ninguna
                lbl_Orden.Enabled = True
                txt_FechaEstimada.Enabled = False
                txt_FechaPago.Enabled = False
                btn_CambiaCuenta.Visible = False
                lbl_EtiqBanco.Visible = True
                lbl_TextoOP.Enabled = False
                lbl_Anula.Visible = False
                ddl_ConceptoAnula.Visible = False

                btn_Consultar.Visible = IIf(Consulta = 0, False, True)
                btn_Anular.Visible = IIf(Baja = 0, False, True)

                btn_Guardar.Visible = False
                btn_Cancelar.Visible = False

        End Select
    End Sub

    Private Sub EdoCapturaMontos(blnEstado As Boolean)
        'Si son devoluciones
        If hid_devolucion.Value = -1 Then blnEstado = False

        For Each row In gvd_Contabilidad.Rows
            DirectCast(row.FindControl("txt_prcPrima"), TextBox).Enabled = blnEstado
            DirectCast(row.FindControl("txt_Prima"), TextBox).Enabled = blnEstado
        Next
    End Sub

    Private Function ValidaCampoRequerido() As Boolean
        ValidaCampoRequerido = False

        If lbl_Orden.Text = vbNullString Then
            Mensaje.MuestraMensaje(Titulo, "Debe especificar la Orden de Pago", TipoMsg.Advertencia)
            Exit Function
        End If
        ValidaCampoRequerido = True
    End Function

    'Private Sub btn_Modificar_Click(sender As Object, e As EventArgs) Handles btn_Modificar.Click
    '    Try
    '        If ValidaCampoRequerido() Then
    '            If Consulta(lbl_Orden.Text) Then
    '                EstadoOP = Operacion.Modifica
    '                LLenaControl()

    '                If ValidaEstatusOP() = False Then
    '                    EstadoOP = Operacion.Consulta
    '                End If

    '                EdoControlOP(EstadoOP)
    '            Else
    '                Mensaje.MuestraMensaje(Master.Titulo, "La Orden de Pago no existe", TipoMsg.Advertencia)
    '            End If
    '        End If
    '    Catch ex As Exception
    '        Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
    '        Funciones.fn_InsertaExcepcion(cod_modulo, 0, Master.cod_usuario, "btn_Modificar_Click: " & ex.Message)
    '    End Try
    'End Sub

    Private Sub btn_Anular_Click(sender As Object, e As EventArgs) Handles btn_Anular.Click
        Try
            If ValidaCampoRequerido() Then
                If fn_ConsultaOP(lbl_Orden.Text) Then
                    EstadoOP = Operacion.Anula
                    LLenaControl()

                    If ValidaEstatusOP() = False Then
                        EstadoOP = Operacion.Consulta
                    End If

                    EdoControl(EstadoOP)
                Else
                    Mensaje.MuestraMensaje(Titulo, "La Orden de Pago no existe", TipoMsg.Advertencia)
                End If
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Anular_Click: " & ex.Message)
        End Try
    End Sub

    Private Function ValidaEstatusOP() As Boolean

        If Val(txt_Estatus.Text) > 2 Then
            EdoControl(Operacion.Consulta)
            Mensaje.MuestraMensaje(Titulo, "El estatus de esta Órden de Pago es: " & txt_Estatus.Text & ", por lo tanto no puede ser modificada o anulada", TipoMsg.Advertencia)
            Return False
        End If

        If IsDate(lbl_FecAnula.Text) Then
            EdoControl(Operacion.Consulta)
            Mensaje.MuestraMensaje(Titulo, "Esta Orden de Pago fue anulada con anterioridad, solo puede ser consultada", TipoMsg.Advertencia)
            Return False
        End If

        If IsDate(lbl_FecContabilidad.Text) Then
            EdoControl(Operacion.Consulta)
            Mensaje.MuestraMensaje(Titulo, "Esta Orden de Pago ya fue autorizada por Contabilidad, solo puede ser consultada", TipoMsg.Advertencia)
            Return False
        End If

        If IsDate(lbl_FecTesoreria.Text) Then
            Mensaje.MuestraMensaje(Titulo, "Esta Orden de Pago ya fue autorizada por Tesoreria, al modificarla perdera dicha autorización", TipoMsg.Advertencia)
            Return True
        End If

        Return True
    End Function

    Private Sub btn_Consultar_Click(sender As Object, e As EventArgs) Handles btn_Consultar.Click
        Try
            If ValidaCampoRequerido() Then
                If fn_ConsultaOP(lbl_Orden.Text) Then
                    EstadoOP = Operacion.Consulta
                    LLenaControl()
                    EdoControl(Operacion.Consulta)
                Else
                    Mensaje.MuestraMensaje(Titulo, "La Orden de Pago no existe", TipoMsg.Advertencia)
                End If
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Consultar_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Cancelar_Click(sender As Object, e As EventArgs) Handles btn_Cancelar.Click
        Try
            EstadoOP = Operacion.Ninguna
            LimpiaControl()
            EdoControl(EstadoOP)
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Cancelar_Click: " & ex.Message)
        End Try
    End Sub

    Public Sub ConsultaOrdenPago(ByVal nro_op As Integer)
        If fn_ConsultaOP(nro_op) Then
            EstadoOP = Operacion.Consulta
            LLenaControl()
            EdoControl(Operacion.Consulta)

            hid_Endoso.Value = ""
            hid_Capa.Value = ""
            hid_Ramo.Value = ""
            hid_Contrato.Value = ""
            hid_Compañia.Value = ""
            hid_Cuota.Value = ""

            Funciones.AbrirModal("#OrdenPago")
        End If
    End Sub

    Protected Sub DespliegaFiltro(sender As Object, e As ImageClickEventArgs)
        Try
            Dim elemento As String
            Dim Datos As New ArrayList()
            Dim filtrados() As String = {""}
            Dim blnTodo As Boolean = True
            Dim Generales() As String = Split(sender.AlternateText, "|")
            Dim Consulta As String = "id_pv > '0'"

            hid_Filtro.Value = Generales(2)

            Dim ArrayOrden() As String = Split(hid_OrdenFiltrado.Value, "|")
            Dim Indice As Integer = Array.IndexOf(ArrayOrden, hid_Filtro.Value)

            For i = 1 To ArrayOrden.Length - 1
                If hid_Filtro.Value <> ArrayOrden(i) Then
                    Select Case ArrayOrden(i)
                        Case Filtros.Poliza
                            Consulta = Consulta & ArmaConsulta("poliza", Split(hid_Endoso.Value, "|"))
                        Case Filtros.Capa
                            Consulta = Consulta & ArmaConsulta("nro_layer", Split(hid_Capa.Value, "|"))
                        Case Filtros.Ramo
                            Consulta = Consulta & ArmaConsulta("ramo_contable", Split(hid_Ramo.Value, "|"))
                        Case Filtros.Contrato
                            Consulta = Consulta & ArmaConsulta("id_contrato", Split(hid_Contrato.Value, "|"))
                        Case Filtros.Compañia
                            Consulta = Consulta & ArmaConsulta("compañia", Split(hid_Compañia.Value, "|"))
                        Case Filtros.Cuota
                            Consulta = Consulta & ArmaConsulta("nro_cuota", Split(hid_Cuota.Value, "|"))
                    End Select
                End If
            Next


            Select Case hid_Filtro.Value
                Case Filtros.Poliza
                    filtrados = Split(hid_Endoso.Value, "|")
                Case Filtros.Capa
                    filtrados = Split(hid_Capa.Value, "|")
                Case Filtros.Ramo
                    filtrados = Split(hid_Ramo.Value, "|")
                Case Filtros.Contrato
                    filtrados = Split(hid_Contrato.Value, "|")
                Case Filtros.Compañia
                    filtrados = Split(hid_Compañia.Value, "|")
                Case Filtros.Cuota
                    filtrados = Split(hid_Cuota.Value, "|")
            End Select


            'Titulo de la Ventana
            lbl_TituloFiltro.Text = "Filtrado " & Generales(1)

            chk_Filtro.Items.Clear()

            Dim myRow() As Data.DataRow
            myRow = dtContabilidad.Select(Consulta)

            Dim dtDatos As DataTable
            dtDatos = GeneraDTContabilidad()

            For Each item In myRow
                dtDatos.Rows.Add(item("nro_reas"), item("id_imputacion"), item("txt_clave"), item("txt_clave_isr"), item("cod_cpto_pri"), item("cod_deb_cred_pri"), item("prima_cedida"), item("pje_pri"),
                                 item("prima"), item("cod_cpto_com"), item("cod_deb_cred_com"), item("comisiones"), item("pje_com"), item("comision"), item("prima_neta"), item("pje_isr"),
                                 item("monto_isr"), item("cod_broker"), item("broker"), item("cod_cia"), item("compañia"), item("cod_profit_center"), item("cod_subprofit_center"), item("id_contrato"),
                                 item("nro_tramo"), item("id_pv"), item("Poliza"), item("cod_suc_stro"), item("aaaa_ejercicio_stro"), item("nro_stro"), item("aaaamm_movimiento"), item("cod_ramo_contable"),
                                 item("ramo_contable"), item("nro_cuota"), item("fecha_fac"), item("cod_major"), item("cod_minor"), item("cod_class_peril"), item("sn_ogis"), item("nro_layer"), item("monto_isr_dev"))
            Next

            Dim ArrayElementos = dtDatos.AsEnumerable().[Select](Function(c) CType(c(Generales(0)), System.String)).Distinct().ToList()

            For Each elemento In ArrayElementos
                If Len(elemento) = 0 Then elemento = "(VACIAS)"
                chk_Filtro.Items.Add(elemento)
            Next

            If Len(filtrados(0)) > 0 Then
                blnTodo = False
            End If

            For Each item In chk_Filtro.Items
                If filtrados.Contains(item.Text) Or blnTodo = True Then
                    item.Selected = True
                End If
            Next

            Funciones.AbrirModal("#Filtros")
            Funciones.EjecutaFuncion("fn_Desplazable('#Filtro');")

        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "DespliegaFiltro: " & ex.Message)
        End Try
    End Sub

    Private Function GeneraDTContabilidad() As DataTable
        GeneraDTContabilidad = New DataTable
        With GeneraDTContabilidad
            .Columns.Add("nro_reas")
            .Columns.Add("id_imputacion")
            .Columns.Add("txt_clave")
            .Columns.Add("txt_clave_isr")
            .Columns.Add("cod_cpto_pri")
            .Columns.Add("cod_deb_cred_pri")
            .Columns.Add("prima_cedida")
            .Columns.Add("pje_pri")
            .Columns.Add("prima")
            .Columns.Add("cod_cpto_com")
            .Columns.Add("cod_deb_cred_com")
            .Columns.Add("comisiones")
            .Columns.Add("pje_com")
            .Columns.Add("comision")
            .Columns.Add("prima_neta")
            .Columns.Add("pje_isr")
            .Columns.Add("monto_isr")
            .Columns.Add("cod_broker")
            .Columns.Add("broker")
            .Columns.Add("cod_cia")
            .Columns.Add("compañia")
            .Columns.Add("cod_profit_center")
            .Columns.Add("cod_subprofit_center")
            .Columns.Add("id_contrato")
            .Columns.Add("nro_tramo")
            .Columns.Add("id_pv")
            .Columns.Add("Poliza")
            .Columns.Add("cod_suc_stro")
            .Columns.Add("aaaa_ejercicio_stro")
            .Columns.Add("nro_stro")
            .Columns.Add("aaaamm_movimiento")
            .Columns.Add("cod_ramo_contable")
            .Columns.Add("ramo_contable")
            .Columns.Add("nro_cuota")
            .Columns.Add("fecha_fac")
            .Columns.Add("cod_major")
            .Columns.Add("cod_minor")
            .Columns.Add("cod_class_peril")
            .Columns.Add("sn_ogis")
            .Columns.Add("nro_layer")
            .Columns.Add("monto_isr_dev")
        End With
    End Function

    Private Sub btn_aceptar_filtro_Click(sender As Object, e As EventArgs) Handles btn_aceptar_filtro.Click
        Try
            Dim blnTodo As Boolean = True

            Dim hid_Control As HiddenField
            hid_Control = New HiddenField

            Select Case hid_Filtro.Value
                Case Filtros.Poliza
                    hid_Control = hid_Endoso
                Case Filtros.Capa
                    hid_Control = hid_Capa
                Case Filtros.Ramo
                    hid_Control = hid_Ramo
                Case Filtros.Contrato
                    hid_Control = hid_Contrato
                Case Filtros.Compañia
                    hid_Control = hid_Compañia
                Case Filtros.Cuota
                    hid_Control = hid_Cuota
            End Select

            Dim ArrayOrden() As String = Split(hid_OrdenFiltrado.Value, "|")
            Dim Indice As Integer = Array.IndexOf(ArrayOrden, hid_Filtro.Value)

            If Indice = -1 Then
                hid_OrdenFiltrado.Value = hid_OrdenFiltrado.Value & "|" & hid_Filtro.Value
                ReDim Preserve ArrayOrden(ArrayOrden.Length)
                ArrayOrden(ArrayOrden.Length - 1) = hid_Filtro.Value
                Indice = ArrayOrden.Length - 1
            End If

            hid_Control.Value = ""

            For Each item In chk_Filtro.Items
                If item.Selected = True Then
                    hid_Control.Value = hid_Control.Value & IIf(Len(hid_Control.Value) > 0, "|", "") & item.Text
                Else
                    blnTodo = False
                End If
            Next

            If blnTodo = True Then
                hid_Control.Value = ""
            End If


            Dim Consulta As String = "id_pv > '0'"
            Consulta = Consulta & ArmaConsulta("poliza", Split(hid_Endoso.Value, "|"))
            Consulta = Consulta & ArmaConsulta("nro_layer", Split(hid_Capa.Value, "|"))
            Consulta = Consulta & ArmaConsulta("ramo_contable", Split(hid_Ramo.Value, "|"))
            Consulta = Consulta & ArmaConsulta("id_Contrato", Split(hid_Contrato.Value, "|"))
            Consulta = Consulta & ArmaConsulta("compañia", Split(hid_Compañia.Value, "|"))
            Consulta = Consulta & ArmaConsulta("nro_cuota", Split(hid_Cuota.Value, "|"))

            Dim myRow() As Data.DataRow
            myRow = dtContabilidad.Select(Consulta)

            Dim dtDatos As DataTable
            dtDatos = GeneraDTContabilidad()

            For Each item In myRow
                dtDatos.Rows.Add(item("nro_reas"), item("id_imputacion"), item("txt_clave"), item("txt_clave_isr"), item("cod_cpto_pri"), item("cod_deb_cred_pri"), item("prima_cedida"), item("pje_pri"),
                                 item("prima"), item("cod_cpto_com"), item("cod_deb_cred_com"), item("comisiones"), item("pje_com"), item("comision"), item("prima_neta"), item("pje_isr"),
                                 item("monto_isr"), item("cod_broker"), item("broker"), item("cod_cia"), item("compañia"), item("cod_profit_center"), item("cod_subprofit_center"), item("id_contrato"),
                                 item("nro_tramo"), item("id_pv"), item("Poliza"), item("cod_suc_stro"), item("aaaa_ejercicio_stro"), item("nro_stro"), item("aaaamm_movimiento"), item("cod_ramo_contable"),
                                 item("ramo_contable"), item("nro_cuota"), item("fecha_fac"), item("cod_major"), item("cod_minor"), item("cod_class_peril"), item("sn_ogis"), item("nro_layer"), item("monto_isr_dev"))
            Next

            Funciones.LlenaGrid(gvd_Contabilidad, dtDatos)

            btn_Poliza.CssClass = IIf(Len(hid_Endoso.Value) > 0, "btn-filtro-verde", "btn-filtro")
            btn_Capa.CssClass = IIf(Len(hid_Capa.Value) > 0, "btn-filtro-verde", "btn-filtro")
            btn_Ramo.CssClass = IIf(Len(hid_Ramo.Value) > 0, "btn-filtro-verde", "btn-filtro")
            btn_Contrato.CssClass = IIf(Len(hid_Contrato.Value) > 0, "btn-filtro-verde", "btn-filtro")
            btn_Compañia.CssClass = IIf(Len(hid_Compañia.Value) > 0, "btn-filtro-verde", "btn-filtro")
            btn_Cuota.CssClass = IIf(Len(hid_Cuota.Value) > 0, "btn-filtro-verde", "btn-filtro")

            EdoControl(EstadoOP)

            Totales_Contabilidad()

        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_aceptar_filtro_Click: " & ex.Message)
        End Try
    End Sub

    Public Function ArmaConsulta(ByVal campo As String, ByVal elementos() As String) As String
        Dim Consulta As String = ""

        For Each item In elementos
            If Len(item) > 0 Then
                Consulta = Consulta & IIf(Len(Consulta) > 0, " OR ", " AND (") & campo & " = '" & item & "'"
            End If
        Next

        If Len(Consulta) > 0 Then
            Consulta = Consulta & ")"
        End If

        Return Consulta
    End Function

    Private Sub btn_NingunoFiltro_Click(sender As Object, e As EventArgs) Handles btn_NingunoFiltro.Click
        Try
            For Each item In chk_Filtro.Items
                item.Selected = False
            Next
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_NingunoFiltro_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_TodosFiltro_Click(sender As Object, e As EventArgs) Handles btn_TodosFiltro.Click
        Try
            For Each item In chk_Filtro.Items
                item.Selected = True
            Next
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_TodosFiltro_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Cobranzas_Click(sender As Object, e As EventArgs) Handles btn_Cobranzas.Click
        Try
            Dim arrayPoliza(0) As String
            Dim intElemento = 0
            Dim poliza() As String

            If gvd_Contabilidad.Rows.Count > 0 Then
                For Each row In gvd_Contabilidad.Rows
                    poliza = Split(gvd_Contabilidad.DataKeys(row.RowIndex)("poliza"), "-")
                    If Not arrayPoliza.Contains(poliza(0) & "-" & poliza(1) & "-" & poliza(2) & "-" & poliza(3)) Then
                        ReDim Preserve arrayPoliza(intElemento)
                        arrayPoliza(intElemento) = poliza(0) & "-" & poliza(1) & "-" & poliza(2) & "-" & poliza(3)
                        intElemento = intElemento + 1
                    End If
                Next

                DetalleCobranzas(0, arrayPoliza, 0)

                Funciones.AbrirModal("#CobranzasModal")
            Else
                Mensaje.MuestraMensaje(Titulo, "No es posible ver el Detalle de Cobranza de una Orden de Pago anulada", TipoMsg.Falla)
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Cobranzas_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Guardar_Click(sender As Object, e As EventArgs) Handles btn_Guardar.Click
        Try

            If EstadoOP = Operacion.Anula Then
                If Funciones.fn_Ejecuta("spD_OrdenPago " & lbl_Orden.Text & ",0,'" & cod_usuario & "',0," & ddl_ConceptoAnula.SelectedValue & ",0,0") = 1 Then

                    If fn_ConsultaOP(lbl_Orden.Text) Then
                        EstadoOP = Operacion.Consulta
                        LLenaControl()
                        EdoControl(Operacion.Consulta)
                    End If

                    'Actualiza el Repositorio
                    Funciones.fn_Guarda_OP(lbl_Orden.Text, user, Eramake.eCryptography.Decrypt(pws), False, False)

                    Mensaje.MuestraMensaje(Titulo, "La Orden de pago ha sido anulada", TipoMsg.Confirma)
                End If
            End If



        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Guardar_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Imprimir_Click(sender As Object, e As EventArgs) Handles btn_Imprimir.Click
        Try
            Funciones.fn_Imprime_OP(url_reportes, lbl_Orden.Text)
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Imprimir_Click: " & ex.Message)
        End Try
    End Sub

    Public Sub MuestraRiesgosPoliza(ByVal cod_suc As Integer, ByVal cod_ramo As Integer, ByVal nro_pol As Integer, ByVal aaaa_endoso As Integer, ByVal nro_endoso As Integer, Optional ByVal Asegurado As String = vbNullString)
        Dim ws As New ws_MesaControl.MesaControlClient

        Dim dtUbicaciones = New DataTable
        dtUbicaciones = Funciones.Lista_A_Datatable(ws.ObtieneUbicacionesPoliza(cod_suc, cod_ramo, nro_pol, aaaa_endoso, nro_endoso).ToList)

        Dim dtRiesgosPol = New DataTable
        dtRiesgosPol = Funciones.Lista_A_Datatable(ws.ObtieneRiesgoPoliza(cod_suc, cod_ramo, nro_pol, aaaa_endoso, nro_endoso, -1, "").ToList)
        If dtRiesgosPol.Rows.Count > 0 Then

            'Funciones.LlenaGrid(gvd_RiesgosPoliza, dtRiesgosPol)

            Funciones.LlenaDDL(ddl_Ubicacion, dtUbicaciones, "cod_item", "cod_item")

            lbl_RiesgoPoliza.Text = cod_suc & "-" & cod_ramo & "-" & nro_pol & "-" & aaaa_endoso & "-" & nro_endoso
            lbl_AseguradoRiesgo.Text = Asegurado

            ddl_Ubicacion.SelectedIndex = ddl_Ubicacion.Items.Count - 1
            ddl_Ubicacion_SelectedIndexChanged(ddl_Ubicacion, Nothing)

            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("chk_Sel"), CheckBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_Ubicacion"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_ClaveRamo"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_SearchRamo"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_ClaveSubRamo"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_SearchSubramo"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_ClaveSeccion"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_SearchSeccion"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_ClaveCobertura"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_SearchCobertura"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("chk_Facultativo"), CheckBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("chk_Adicional"), CheckBox).Visible = False


            Funciones.AbrirModal("#RiesgosPoliza")

        End If
    End Sub

    Protected Sub ddl_Ubicacion_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            Dim ws As New ws_MesaControl.MesaControlClient

            Dim poliza() = Split(lbl_RiesgoPoliza.Text, "-")

            Dim dtRiesgosPol = New DataTable
            dtRiesgosPol = Funciones.Lista_A_Datatable(ws.ObtieneRiesgoPoliza(poliza(0), poliza(1), poliza(2), poliza(3), poliza(4), sender.selectedValue, "").ToList)

            Funciones.LlenaGrid(gvd_RiesgosPoliza, dtRiesgosPol)

            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("chk_Sel"), CheckBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_Ubicacion"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_ClaveRamo"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_SearchRamo"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_ClaveSubRamo"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_SearchSubramo"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_ClaveSeccion"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_SearchSeccion"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_ClaveCobertura"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("txt_SearchCobertura"), TextBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("chk_Facultativo"), CheckBox).Visible = False
            CType(gvd_RiesgosPoliza.Rows(gvd_RiesgosPoliza.Rows.Count - 1).FindControl("chk_Adicional"), CheckBox).Visible = False

        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "ddl_Ubicacion_SelectedIndexChanged: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_CerrarRie_Click(sender As Object, e As EventArgs) Handles btn_CerrarRie.Click
        Try
            Funciones.LlenaGrid(gvd_RiesgosPoliza, Nothing)
            Funciones.CerrarModal("#RiesgosPoliza")
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_CerrarRie_Click: " & ex.Message)
        End Try
    End Sub

    Protected Sub chk_EndosoCobranza(sender As Object, e As EventArgs)
        Try
            Dim row As GridViewRow = DirectCast(DirectCast(DirectCast(sender, CheckBox).Parent, DataControlFieldCell).Parent, GridViewRow)
            Dim id_pv As Integer = gvd_Endosos.DataKeys(row.RowIndex)("id_pv")

            If sender.checked = True Then
                Select Case sender.id
                    Case "chk_Financiado"
                        TryCast(row.FindControl("chk_Pago"), CheckBox).Checked = False
                        TryCast(row.FindControl("chk_Recargo"), CheckBox).Checked = False
                    Case "chk_Pago", "chk_Recargo"
                        TryCast(row.FindControl("chk_Financiado"), CheckBox).Checked = False
                End Select
            End If

            Funciones.fn_Ejecuta("spU_EndosoCobranza " & id_pv & "," &
                                                         CInt(TryCast(row.FindControl("chk_Financiado"), CheckBox).Checked) & "," &
                                                         CInt(TryCast(row.FindControl("chk_Pago"), CheckBox).Checked) & "," &
                                                         CInt(TryCast(row.FindControl("chk_Recargo"), CheckBox).Checked) & ",'" &
                                                         cod_usuario & "',0")



        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "chk_EndosoCobranza: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_ImprimeCobranza_Click(sender As Object, e As EventArgs) Handles btn_ImprimeCobranza.Click
        Try
            Dim sn_impresion As Boolean = False
            For Each row In gvd_Endosos.Rows
                If TryCast(row.FindControl("chk_Sel"), CheckBox).Checked = True Then
                    sn_impresion = True
                    Dim server As String = Replace(Replace(url_reportes, "@Reporte", "CobranzaOP"), "@Formato", "PDF")
                    Funciones.EjecutaFuncion("fn_Imprime_Reporte('" & server & "&str_pol=+" & ddl_PolizaCobranzas.SelectedValue & "+&nro_endoso=" & gvd_Endosos.DataKeys(row.RowIndex)("nro_endoso") & "&nro_op=N/A');", "ImpresionCobranza" & gvd_Endosos.DataKeys(row.RowIndex)("nro_endoso"))
                End If
            Next

            If sn_impresion = False Then
                Mensaje.MuestraMensaje(Titulo, "No se ha seleccionado ningún Endoso para impresión", TipoMsg.Falla)
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_ImprimeCobranza_Click: " & ex.Message)
        End Try
    End Sub

    Public Sub EvaluaPermisosModulo()
        Dim dtPermisos As New DataTable
        Funciones.fn_Consulta("spS_SubMenuWeb '" & cod_usuario & "'," & cod_modulo & "," & cod_submodulo, dtPermisos)

        Alta = 0
        Baja = 0
        Cambio = 0
        Consulta = 0

        If dtPermisos.Rows.Count > 0 Then
            Alta = dtPermisos.Rows(0)("sn_alta")
            Baja = dtPermisos.Rows(0)("sn_baja")
            Cambio = dtPermisos.Rows(0)("sn_cambio")
            Consulta = dtPermisos.Rows(0)("sn_consult")
        End If

    End Sub

    Private Sub gvd_Autorizacion_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvd_Autorizacion.RowCommand
        Try
            If e.CommandName = "CambioPin" Then
                Dim Index As Integer = e.CommandSource.NamingContainer.RowIndex


                txt_Usuario.Text = gvd_Autorizacion.DataKeys(Index)("usuario")
                hid_CodRol.Value = gvd_Autorizacion.DataKeys(Index)("cod_rol")
                txt_Rol.Text = TryCast(gvd_Autorizacion.Rows(Index).FindControl("txt_Rol"), TextBox).Text
                txt_UsuarioSII.Text = gvd_Autorizacion.DataKeys(Index)("cod_usuario")
                txt_UsuarioNT.Text = TryCast(gvd_Autorizacion.Rows(Index).FindControl("txt_Usuario"), TextBox).Text

                Funciones.AbrirModal("#PinCode")
                Funciones.CerrarModal("#Autorizacion")
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "gvd_Autorizacion_RowCommand: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Cerrar_Pin_Click(sender As Object, e As EventArgs) Handles btn_Cerrar_Pin.Click
        Try
            Funciones.AbrirModal("#Autorizacion")
            Funciones.CerrarModal("#PinCode")
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Cerrar_Pin_Click: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_Cambia_Pin_Click(sender As Object, e As EventArgs) Handles btn_Cambia_Pin.Click
        Try
            Dim ws As New ws_Generales.GeneralesClient
            Dim dtUsuario As New DataTable
            Dim pin_code As String = vbNullString

            If Funciones.IsAuthenticated("GMX.COM.MX", txt_UsuarioNT.Text, txt_Password.Text) = False Then
                Mensaje.MuestraMensaje(Titulo, "La contraseña de red es incorrecta", TipoMsg.Falla)
                Exit Sub
            Else
                pin_code = Eramake.eCryptography.Encrypt(txt_pin_ant.Text)

                If Funciones.fn_ObtieneUsuarioRol(hid_CodRol.Value, dtUsuario, 0, txt_UsuarioSII.Text).Rows.Count > 0 Then
                    If pin_code <> dtUsuario.Rows(0)("pin_code") Then
                        Mensaje.MuestraMensaje(Titulo, "El pin_code actual es incorrecto", TipoMsg.Falla)
                        Exit Sub
                    End If
                Else
                    Mensaje.MuestraMensaje(Titulo, "El usuario no cuenta con PIN_CODE", TipoMsg.Falla)
                    Exit Sub
                End If
            End If

            Funciones.fn_Ejecuta("spU_PIN_CODE '" & txt_UsuarioSII.Text & "','" & Eramake.eCryptography.Encrypt(txt_pin_nuevo.Text) & "'")

            Mensaje.MuestraMensaje(Titulo, "El PIN_CODE fue actualizado con exito", TipoMsg.Confirma)

            Funciones.AbrirModal("#Autorizacion")
            Funciones.CerrarModal("#PinCode")
        Catch ex As Exception
            Mensaje.MuestraMensaje(Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(cod_modulo, cod_submodulo, cod_usuario, "btn_Cerrar_Pin_Click: " & ex.Message)
        End Try
    End Sub

    'JJIMENEZ
    'Evento que evalua los elementos seleccionados en cuentas bancarias
    Private Sub btnTransferenciasBancariasAceptar_stro_Click(sender As Object, e As EventArgs) Handles btnTransferenciasBancariasAceptar_stro.Click

        Dim oCampoOculto As HiddenField

        Dim oTxt As TextBox

        Dim oCmb As DropDownList

        Dim oListaElementos As List(Of String)

        Dim oParametros As New Dictionary(Of String, Object)

        Dim oDatos As DataSet

        Dim numBanco As String

        Try

            If txtCuentaBancariaT_stro.Text = txtCuentaBancariaT_stro_Confirmacion.Text Then

                numBanco = txtCuentaBancariaT_stro.Text
                numBanco = numBanco.Substring(0, 3)

                If numBanco < 100 And numBanco > 10 Then
                    numBanco = numBanco.Substring(numBanco.Length - 2)
                    cmbBancoT_stro.SelectedValue = numBanco
                Else
                    If numBanco < 10 Then
                        numBanco = numBanco.Substring(numBanco.Length - 1)
                        cmbBancoT_stro.SelectedValue = numBanco
                    Else
                        cmbBancoT_stro.SelectedValue = numBanco
                    End If
                End If

                oParametros = New Dictionary(Of String, Object)

                oParametros.Add("CodigoBanco", CInt(cmbBancoT_stro.SelectedValue))
                oParametros.Add("CuentaClabe", CStr(txtCuentaBancariaT_stro.Text.Trim))

                oDatos = New DataSet
                oDatos = Funciones.ObtenerDatos("usp_ValidarCuentaClabe", oParametros)

                If oDatos Is Nothing OrElse oDatos.Tables(0).Rows.Count = 0 Then
                    Mensaje.MuestraMensaje("Cuentas bancarias", "Error al validar la cuenta bancaria", TipoMsg.Falla)
                    Return
                ElseIf Not IsNumeric(txtCuentaBancariaT_stro.Text.Trim) OrElse oDatos.Tables(0).Rows(0).Item("Valido") = "N" Then
                    Mensaje.MuestraMensaje("Cuentas bancarias", "La cuenta bancaria no es válida para el banco seleccionado, verifique que la cuenta conste de 18 dígitos.", TipoMsg.Advertencia)
                    Return
                End If

                'TextBox
                oListaElementos = New List(Of String)

                oListaElementos.Add("SucursalT_stro")
                oListaElementos.Add("BeneficiarioT_stro")
                oListaElementos.Add("CuentaBancariaT_stro")
                oListaElementos.Add("PlazaT_stro")
                oListaElementos.Add("AbaT_stro")

                For Each oElemento In oListaElementos

                    oCampoOculto = TryCast(cph_principal.FindControl(String.Format("o{0}", oElemento)), HiddenField)

                    If Not oCampoOculto Is Nothing Then

                        oTxt = TryCast(pnlDatosBanco_stro.FindControl(String.Format("txt{0}", oElemento)), TextBox)

                        If Not oTxt Is Nothing Then
                            oCampoOculto.Value = oTxt.Text.ToString.Trim
                        End If

                    End If

                Next

                'ComboBox
                oListaElementos = New List(Of String)

                oListaElementos.Add("BancoT_stro")
                oListaElementos.Add("MonedaT_stro")
                oListaElementos.Add("TipoCuentaT_stro")

                For Each oElemento In oListaElementos

                    oCampoOculto = TryCast(cph_principal.FindControl(String.Format("o{0}", oElemento)), HiddenField)

                    If Not oCampoOculto Is Nothing Then

                        oCmb = TryCast(pnlDatosBanco_stro.FindControl(String.Format("cmb{0}", oElemento)), DropDownList)

                        If Not oCmb Is Nothing AndAlso oCmb.Items.Count > 0 Then
                            oCampoOculto.Value = oCmb.SelectedValue.ToString
                        End If

                    End If

                Next
                'FJCP MEJORAS 10290 PAGO INTERNACIONAL INI
                If HiddenFieldPI.Value <> "" Then
                    Funciones.EjecutaFuncion("llenarCpto2();")
                End If
                'FJCP MEJORAS 10290 PAGO TESOFE FIN
                If txtCuentaBancariaT_stro.Text = Funciones.fn_EjecutaStr("usp_CargarDatosBancariosDepGob_stro @Accion = 2").ToString() Then
                    MuestraMensaje("Dependencias", "Seleccione una dependencia", TipoMsg.Advertencia)
                End If

                Funciones.CerrarModal("#Transferencias_stro")
            Else
                'Mensaje.MuestraMensaje("Cuentas bancarias", "Error al validar la confirmacion de la cuenta bancaria", TipoMsg.Falla)
                Mensaje.MuestraMensaje("Cuentas bancarias", "La cuenta bancaria y la confirmación de la cuenta no coinciden.", TipoMsg.Falla)
                txtCuentaBancariaT_stro_Confirmacion.Text = String.Empty
                txtCuentaBancariaT_stro.Text = String.Empty
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje("Master Page", String.Format("btnTransferenciasBancariasAceptar_stro_Click Error: {0}" + " Cuenta bancaria sin banco asosiado " + txtCuentaBancariaT_stro.Text, ex.Message), TipoMsg.Falla)
        End Try

    End Sub

    'JJIMENEZ
    Private Sub btnRegistrarTercero_stro_Click(sender As Object, e As EventArgs) Handles btnRegistrarTercero_stro.Click

        Dim oParametros As Dictionary(Of String, Object)

        Dim oDatos = New DataSet

        Dim ListaTerceros As StringBuilder

        Dim Genericos = New String() {"XEXX010101000", "XAXX010101000"}

        Dim oCodigoTercero, oNombreTercero, oRFCTercero As TextBox

        Try

            ListaTerceros = New StringBuilder

            ListaTerceros.AppendFormat("<terceros>{0}", Environment.NewLine)
            ListaTerceros.AppendFormat("<tercero>{0}", Environment.NewLine)

            If (Me.txtRFCRT_stro.Text.Trim.Length = 12 OrElse Genericos.Contains(Me.txtRFCRT_stro.Text.Trim())) AndAlso
               (String.IsNullOrWhiteSpace(Me.txtApellidoPaternoRT_stro.Text.Trim) OrElse String.IsNullOrWhiteSpace(Me.txtApellidoPaternoRT_stro.Text.Trim())) Then

                'Es judírico

                ListaTerceros.AppendFormat("<Nombre>{0}</Nombre>{1}", Me.txtNombreRT_stro.Text.Trim, Environment.NewLine)
                ListaTerceros.AppendFormat("<ApellidoPaterno>{0}</ApellidoPaterno>{1}", Me.txtNombreRT_stro.Text.Trim, Environment.NewLine)
                ListaTerceros.AppendFormat("<ApellidoMaterno>{0}</ApellidoMaterno>{1}", String.Empty, Environment.NewLine)
                ListaTerceros.AppendFormat("<TipoPersona>{0}</TipoPersona>{1}", "F", Environment.NewLine)

            Else

                'Es física

                ListaTerceros.AppendFormat("<Nombre>{0}</Nombre>{1}", Me.txtNombreRT_stro.Text.Trim, Environment.NewLine)
                ListaTerceros.AppendFormat("<ApellidoPaterno>{0}</ApellidoPaterno>{1}", Me.txtApellidoPaternoRT_stro.Text.Trim, Environment.NewLine)
                ListaTerceros.AppendFormat("<ApellidoMaterno>{0}</ApellidoMaterno>{1}", Me.txtApellidoMaternoRT_stro.Text.Trim, Environment.NewLine)
                ListaTerceros.AppendFormat("<TipoPersona>{0}</TipoPersona>{1}", "F", Environment.NewLine)

            End If

            ListaTerceros.AppendFormat("<RFC>{0}</RFC>{1}", Me.txtRFCRT_stro.Text.Trim, Environment.NewLine)
            ListaTerceros.AppendFormat("</tercero>{0}", Environment.NewLine)

            ListaTerceros.AppendLine("</terceros>")


            oParametros = New Dictionary(Of String, Object)

            oParametros.Add("Datos", ListaTerceros.ToString())
            oParametros.Add("Usuario", IIf(cod_usuario = String.Empty, "JJIMENEZ", cod_usuario))

            oDatos = New DataSet
            oDatos = Funciones.ObtenerDatos("usp_CargarTerceros_stro", oParametros)

            If oDatos Is Nothing OrElse oDatos.Tables(0).Rows.Count = 0 OrElse oDatos.Tables(0).Rows(0).Item("Cargado") = "N" Then
                Funciones.CerrarModal("#CatalogoRegistroTerceros")
                Mensaje.MuestraMensaje("Registro de terceros", "No se ha podido registrar el tercero", TipoMsg.Advertencia)
            Else

                oCodigoTercero = New TextBox
                oNombreTercero = New TextBox

                oCodigoTercero = TryCast(cph_principal.FindControl("txtCodigoBeneficiario_stro"), TextBox)
                oNombreTercero = TryCast(cph_principal.FindControl("txtBeneficiario_stro"), TextBox)
                oRFCTercero = TryCast(cph_principal.FindControl("txtRFC"), TextBox)

                If Not oCodigoTercero Is Nothing AndAlso Not oNombreTercero Is Nothing AndAlso Not oRFCTercero Is Nothing Then

                    With oDatos.Tables(0)
                        oCodigoTercero.Text = .Rows(0)("CodigoTercero")
                        oNombreTercero.Text = String.Format("{0} {1} {2}", .Rows(0)("Nombre"), .Rows(0)("Apellido Paterno"), .Rows(0)("Apellido Materno"))
                        oRFCTercero.Text = .Rows(0)("RFC")
                    End With

                Else
                    Mensaje.MuestraMensaje("Registro de terceros", "Error al ligar tercero registrado", TipoMsg.Advertencia)
                End If

                Funciones.CerrarModal("#CatalogoRegistroTerceros")

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("Master Page", String.Format("btnRegistrarTercero_stro_Click Error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub

    'JJIMENEZ
    Public Sub MuestraTransferenciasBancariasSiniestros(ByVal Control As String, ByVal oBancos As DataTable, ByVal oTiposCuenta As DataTable,
                                                        ByVal oMonedas As DataTable, ByVal oValoresActuales As Dictionary(Of String, Object),
                                                        ByVal bTieneDatosBancarios As Boolean,
                                                        Optional ByVal sn_submod_web As Integer = -1)

        Try
            'VZAVALETA_10290_CC_INI
            If chkPagoInter.Checked Then
                Funciones.AbrirModal("#Transferencias_stro")
                Exit Sub
            End If
            'VZAVALETA_10290_CC_FIN
            'Carga de catalogos de bancos
            If cmbBancoT_stro.Items.Count > 0 Then
                cmbBancoT_stro.Items.Clear()
            End If

            cmbBancoT_stro.DataSource = oBancos
            cmbBancoT_stro.DataTextField = "Descripcion"
            cmbBancoT_stro.DataValueField = "Codigo"
            cmbBancoT_stro.DataBind()

            'Carga de catalogos de tipos de cuenta
            If cmbTipoCuentaT_stro.Items.Count > 0 Then
                cmbTipoCuentaT_stro.Items.Clear()
            End If

            cmbTipoCuentaT_stro.DataSource = oTiposCuenta
            cmbTipoCuentaT_stro.DataTextField = "Descripcion"
            cmbTipoCuentaT_stro.DataValueField = "Codigo"
            cmbTipoCuentaT_stro.DataBind()

            cmbTipoCuentaT_stro.SelectedValue = 2

            'Carga de catalogos de monedas
            If cmbMonedaT_stro.Items.Count > 0 Then
                cmbMonedaT_stro.Items.Clear()
            End If

            cmbMonedaT_stro.DataSource = oMonedas
            cmbMonedaT_stro.DataTextField = "Descripcion"
            cmbMonedaT_stro.DataValueField = "Codigo"
            cmbMonedaT_stro.DataBind()


            Me.txtSucursalT_stro.Text = String.Empty
            Me.txtBeneficiarioT_stro.Text = String.Empty
            Me.txtCuentaBancariaT_stro.Text = String.Empty
            Me.txtCuentaBancariaT_stro_Confirmacion.Text = String.Empty
            Me.txtPlazaT_stro.Text = String.Empty
            Me.txtAbaT_stro.Text = String.Empty

            'se agrega estas validaciones para fasttrack 
            'If bTieneDatosBancarios = True Then
            If Not oValoresActuales Is Nothing Then

                If (oValoresActuales("Fasttrack") = "SI") Then
                    cmbBancoT_stro.SelectedValue = CInt(oValoresActuales("Banco"))
                Else
                    cmbBancoT_stro.SelectedValue = IIf(oValoresActuales("Banco") = String.Empty, cmbBancoT_stro.SelectedValue, oValoresActuales("Banco"))
                End If
                If (oValoresActuales("Fasttrack") = "SI") Then
                    cmbTipoCuentaT_stro.SelectedValue = CInt(oValoresActuales("TipoCuenta"))
                Else
                    cmbTipoCuentaT_stro.SelectedValue = IIf(oValoresActuales("TipoCuenta") = String.Empty, cmbTipoCuentaT_stro.SelectedValue, oValoresActuales("TipoCuenta"))
                End If
                If (oValoresActuales("Fasttrack") = "SI") Then
                    cmbMonedaT_stro.SelectedValue = CInt(oValoresActuales("Moneda"))
                Else
                    cmbMonedaT_stro.SelectedValue = IIf(oValoresActuales("Moneda") = String.Empty, cmbMonedaT_stro.SelectedValue, oValoresActuales("Moneda"))
                End If
                If (oValoresActuales("Fasttrack") = "SI") Then
                    Me.txtSucursalT_stro.Text = oValoresActuales("Sucursal")
                Else
                    Me.txtSucursalT_stro.Text = IIf(oValoresActuales("Sucursal").ToString.Trim = String.Empty, String.Empty, oValoresActuales("Sucursal"))
                End If
                If (oValoresActuales("Fasttrack") = "SI") Then
                    Me.txtBeneficiarioT_stro.Text = oValoresActuales("Beneficiario")
                Else
                    Me.txtBeneficiarioT_stro.Text = IIf(oValoresActuales("Beneficiario").ToString.Trim = String.Empty, String.Empty, oValoresActuales("Beneficiario"))
                End If
                If (oValoresActuales("Fasttrack") = "SI") Then
                    Me.txtCuentaBancariaT_stro.Text = oValoresActuales("CuentaBancaria")
                Else
                    Me.txtCuentaBancariaT_stro.Text = IIf(oValoresActuales("CuentaBancaria").ToString.Trim = String.Empty, String.Empty, oValoresActuales("CuentaBancaria"))
                End If
                If (oValoresActuales("Fasttrack") = "SI") Then
                    Me.txtCuentaBancariaT_stro_Confirmacion.Text = oValoresActuales("CuentaBancaria")
                Else
                    Me.txtCuentaBancariaT_stro_Confirmacion.Text = IIf(oValoresActuales("CuentaBancaria").ToString.Trim = String.Empty, String.Empty, oValoresActuales("CuentaBancaria"))
                End If
                If (oValoresActuales("Fasttrack") = "SI") Then
                    Me.txtPlazaT_stro.Text = oValoresActuales("Plaza")
                Else
                    Me.txtPlazaT_stro.Text = IIf(oValoresActuales("Plaza").ToString.Trim = String.Empty, String.Empty, oValoresActuales("Plaza"))
                End If
                If (oValoresActuales("Fasttrack") = "SI") Then
                    Me.txtAbaT_stro.Text = oValoresActuales("ABA")
                Else
                    Me.txtAbaT_stro.Text = IIf(oValoresActuales("ABA").ToString.Trim = String.Empty, String.Empty, oValoresActuales("ABA"))
                End If

                cmbTipoCuentaT_stro.SelectedValue = 2

                'FJCP 10290 MEJORAS Folio OnBase ini
                If oValoresActuales("tipoUsuario") <> 10 Then
                    If oValoresActuales("fOnbase_edoCta").ToString.Trim <> "" Then
                        Dim hrefOnBase As String
                        linkOnBase.HRef = ""
                        hrefOnBase = ""
                        'cambiar a id 2 cuando se habilite el ws del folio onbase edo cta
                        'hrefOnBase = Funciones.fn_EjecutaStr("usp_consulta_folio_onbase_ws @id = 2,  @folioOnbase = " & oValoresActuales("fOnbase_edoCta").ToString.Trim)
                        hrefOnBase = Funciones.fn_EjecutaStr("usp_consulta_folio_onbase_ws @id_pagar_a =" & oValoresActuales("tipoUsuario") & ",  @folioOnbase = " & oValoresActuales("fOnbase_edoCta").ToString.Trim)
                        linkOnBase.HRef = hrefOnBase
                        lblFoliOnbaseEdoCtaDesc.Visible = True
                        lblFoliOnbaseEdoCta.Visible = True
                        lblFoliOnbaseEdoCta.Text = oValoresActuales("fOnbase_edoCta").ToString.Trim
                        'FJCP 10290 MEJORAS Folio OnBase fin
                    End If
                Else
                    lblFoliOnbaseEdoCtaDesc.Visible = False
                    lblFoliOnbaseEdoCta.Visible = False
                End If

            End If
            'End If


            If bTieneDatosBancarios Then
                cmbBancoT_stro.Enabled = False
                cmbTipoCuentaT_stro.Enabled = False
                cmbMonedaT_stro.Enabled = False
                Me.txtBeneficiarioT_stro.Enabled = False
                Me.txtCuentaBancariaT_stro.Enabled = False
                Me.txtCuentaBancariaT_stro_Confirmacion.Enabled = False
            Else
                cmbBancoT_stro.Enabled = True
                cmbTipoCuentaT_stro.Enabled = True
                cmbMonedaT_stro.Enabled = False
                Me.txtBeneficiarioT_stro.Enabled = True
                Me.txtCuentaBancariaT_stro.Enabled = True
                Me.txtCuentaBancariaT_stro_Confirmacion.Enabled = True
                Me.txtCuentaBancariaT_stro.Text = String.Empty
                Me.txtCuentaBancariaT_stro_Confirmacion.Text = String.Empty
                Me.txtCuentaBancariaT_stro.TextMode = TextBoxMode.Password
                Me.txtCuentaBancariaT_stro_Confirmacion.TextMode = TextBoxMode.Password

                cmbTipoCuentaT_stro.SelectedValue = 2
            End If


            hid_Control.Value = Control
            'FJCP 10290 MEJORAS PAGO INTERNACIONAL 
            hidCtaOriginal.Value = txtCuentaBancariaT_stro.Text.Trim()

            Session.Add("SubModWeb", "")
            Session("SubModWeb") = sn_submod_web

            Funciones.AbrirModal("#Transferencias_stro")
        Catch ex As Exception
            Throw ex
        End Try

    End Sub
    Private Sub drTipoPers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drTipoPers.SelectedIndexChanged
        Dim valor As String
        valor = drTipoPers.SelectedValue
        valor = ""
        If drTipoPers.SelectedValue = "F" Then
            txt_apMatmTer.Enabled = True
            txt_nombresmTer.Enabled = True
        Else
            txt_apMatmTer.Enabled = False
            txt_nombresmTer.Enabled = False
        End If
    End Sub
    Private Sub btnAcepmTer_Click(sender As Object, e As EventArgs) Handles btnAcepmTer.Click
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim dtResult As New DataTable
        'ELIMNAR
        'drTipoPers.SelectedValue = "J"
        'txt_apPatmTer.Text = "SISTRAN TEST"
        'txt_fecNacmTer.Text = "04/04/1995"
        'txtRFCmTer.Text = "SIS950404KY7"
        If ValidaDatos() Then
            Tercero.sApellido1 = txt_apPatmTer.Text.Trim
            Tercero.sApellido2 = txt_apMatmTer.Text.Trim
            Tercero.sNombre = txt_nombresmTer.Text.Trim
            Tercero.sNit = txtRFCmTer.Text.Trim
            Tercero.vFecNacim = Funciones.FormatearFecha(txt_fecNacmTer.Text.Trim, Funciones.enumFormatoFecha.YYYYMMDD)
            Tercero.sTipoPersona = drTipoPers.SelectedValue
            If Tercero.RFC Then
                If drTipoPers.SelectedValue = "F" Then
                    oParametros.Add("txtApellido1", txt_apPatmTer.Text.Trim)
                    oParametros.Add("txtApellido2", txt_apMatmTer.Text.Trim)
                    oParametros.Add("txtNombre", txt_nombresmTer.Text.Trim)
                    oParametros.Add("codTipoDoc", 1)
                    oParametros.Add("nroDoc", txtRFCmTer.Text.Trim)
                    oParametros.Add("nroNit", txtRFCmTer.Text.Trim)
                    oParametros.Add("fecNac", Funciones.FormatearFecha(txt_fecNacmTer.Text.Trim, Funciones.enumFormatoFecha.YYYYMMDD))
                    oParametros.Add("txtSexo", drSexo.SelectedValue)
                    oParametros.Add("codEstCivil", 1)
                    oParametros.Add("codTipoPersona", drTipoPers.SelectedValue)
                    oParametros.Add("txtOrigen", "T")
                    oParametros.Add("codOcupacion", -1)
                    oParametros.Add("codUsuario", cod_usuario.ToString())
                    oParametros.Add("nroEdad", hidEdadmTer.Value)
                    oParametros.Add("dGenerales", 1)
                Else
                    oParametros.Add("txtApellido1", txt_apPatmTer.Text.Trim)
                    oParametros.Add("codTipoDoc", 1)
                    oParametros.Add("nroDoc", txtRFCmTer.Text.Trim)
                    oParametros.Add("nroNit", txtRFCmTer.Text.Trim)
                    oParametros.Add("txtSexo", "F")
                    oParametros.Add("codEstCivil", 1)
                    oParametros.Add("codTipoPersona", drTipoPers.SelectedValue)
                    oParametros.Add("txtOrigen", "T")
                    oParametros.Add("codUsuario", cod_usuario.ToString())
                    oParametros.Add("nroEdad", hidEdadmTer.Value)
                    oParametros.Add("dGenerales", 1)
                End If
                oDatos = Funciones.ObtenerDatos("usp_Alta_Upt_Tercero", oParametros)
                oTabla = oDatos.Tables(0)
                If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                    Dim codError As Integer
                    Dim msgError As String
                    Dim codTerceroNvo As Integer
                    codTerceroNvo = oTabla.Rows(0)("codTercero")
                    codError = oTabla.Rows(0)("snFlag")
                    msgError = oTabla.Rows(0)("msg_err").ToString()
                    If codError <> -1 Then
                        Funciones.CerrarModal("#RegistroTerceros")
                        Funciones.fn_Consulta("spS_CatalogosSIR @strCatalogo = 'BenTercero_stro', @Condicion = " + codTerceroNvo.ToString(), dtResult)
                        hidCodTercero.Value = dtResult.Rows(0)("Clave")
                        hidNomTercero.Value = dtResult.Rows(0)("Descripcion").ToString()
                        hidrfcTercero.Value = dtResult.Rows(0)("OcultaCampo1").ToString()
                        limpRegTerceros()
                        Funciones.EjecutaFuncion("selecTercero();")
                    Else
                        MuestraMensaje("Validación", "Error: " + msgError + ". No se pudo grabar el tercero.", TipoMsg.Advertencia)
                    End If
                End If
            End If
        End If
    End Sub
    Private Function ValidaDatos() As Boolean
        Dim msgError As String
        msgError = ""
        If drTipoPers.SelectedValue = "F" Then
            If Trim(txt_apPatmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Apellido Paterno"
            If Trim(txt_apMatmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Apellido Materno "
            If Trim(txt_nombresmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Nombres "
            If Trim(txt_fecNacmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Fecha Nacimiento "
            If Trim(txtRFCmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "RFC "
        Else
            If Trim(txt_apPatmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Razón Social"
            If Trim(txt_fecNacmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Fecha Nacimiento "
            If Trim(txtRFCmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "RFC "
        End If
        If msgError <> "" Then
            MuestraMensaje("Valida campos", "Los campos " & msgError & "no puedes estar vacíos", TipoMsg.Advertencia)
            Return False
        End If
        Return True
    End Function
    Private Sub btnCancTer_Click(sender As Object, e As EventArgs) Handles btnCancTer.Click
        limpRegTerceros()
        Funciones.CerrarModal("#RegistroTerceros")
    End Sub
    Private Sub limpRegTerceros()
        drTipoPers.SelectedValue = "F"
        txtRFCmTer.Text = ""
        txt_apPatmTer.Text = ""
        txt_apMatmTer.Text = ""
        txt_nombresmTer.Text = ""
        txt_fecNacmTer.Text = ""
    End Sub

    Private Sub chkPagoInter_CheckedChanged(sender As Object, e As EventArgs) Handles chkPagoInter.CheckedChanged
        Dim dt As New DataTable
        Dim A As String
        If chkPagoInter.Checked Then

            'hidCtaOriginal.Value = txtCuentaBancariaT_stro.Text.Trim()

            Funciones.CerrarModal("#Transferencias_stro")
            Funciones.AbrirModal("#PagoInternacional")

            Funciones.fn_Consulta("usp_obt_pais_pago_inter", dt)
            Funciones.LlenaDDL(drPaisPI, dt, "cod_pais", "txt_desc", 0, False)
            drPaisPI.SelectedValue = -1


        Else
            txtCuentaBancariaT_stro.Text = hidCtaOriginal.Value
            txtCuentaBancariaT_stro_Confirmacion.Text = hidCtaOriginal.Value

        End If
    End Sub
    Private Sub btnPagoInternacionalAceptar_Click(sender As Object, e As EventArgs) Handles btnPagoInternacionalAceptar.Click

        Try

            If ValidaDatosPagoInter() Then

                Dim ctaParam As String
                ctaParam = Funciones.fn_EjecutaStr("usp_CargarDatosBancariosDepGob_stro @Accion = 10") 'FJCP MEJORAS PAGO INTERNACIONAL 

                Funciones.CerrarModal("#PagoInternacional")
                Funciones.AbrirModal("#Transferencias_stro")
                'Response.Redirect("Eliminar.aspx")
                'VZAVALETA_10290_CC_INI
                Me.txtCuentaBancariaT_stro.Enabled = False
                Me.txtCuentaBancariaT_stro_Confirmacion.Enabled = False
                Me.txtCuentaBancariaT_stro.TextMode = TextBoxMode.Number
                Me.txtCuentaBancariaT_stro_Confirmacion.TextMode = TextBoxMode.Number

                txtCuentaBancariaT_stro.Text = ctaParam
                txtCuentaBancariaT_stro_Confirmacion.Text = ctaParam
                'VZAVALETA_10290_CC_FIN

            Else
                MuestraMensaje("Validación", "Falta capturar campos", TipoMsg.Falla)
            End If

        Catch ex As Exception

        End Try
    End Sub
    Private Sub btnPagoInternacionalCancelar_Click(sender As Object, e As EventArgs) Handles btnPagoInternacionalCancelar.Click

        txtCuentaBancariaT_stro.Text = hidCtaOriginal.Value
        txtCuentaBancariaT_stro_Confirmacion.Text = hidCtaOriginal.Value
        Funciones.CerrarModal("#PagoInternacional")
        Funciones.AbrirModal("#Transferencias_stro")
        chkPagoInter.Checked = False
        limpiarCtrlesPagoInter()
        limpiarCtrlesTriangulado()
    End Sub

    Private Sub chkTrianguladoPI_CheckedChanged(sender As Object, e As EventArgs) Handles chkTrianguladoPI.CheckedChanged
        If chkTrianguladoPI.Checked Then
            txtTrianNomBancoPI.Enabled = True
            txtTrianCuentaPI.Enabled = True
            txtTrianAbaPI.Enabled = True
        Else
            txtTrianNomBancoPI.Enabled = False
            txtTrianCuentaPI.Enabled = False
            txtTrianAbaPI.Enabled = False
        End If

        limpiarCtrlesTriangulado()
    End Sub

    Private Sub drPaisPI_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drPaisPI.SelectedIndexChanged
        habilitarCntrlesPagoInter()
    End Sub

    Private Sub habilitarCntrlesPagoInter()
        Dim codPais As Integer
        Dim dt As New DataTable
        codPais = drPaisPI.SelectedValue
        Funciones.fn_Consulta("EXECUTE usp_habilita_ctrles_pago_inter @cod_Pais =" + codPais.ToString(), dt)

        limpiarCtrlesPagoInter()

        txtBancoPI.Enabled = IIf(dt.Rows(0)("sn_banco") = -1, True, False)
        txtNroBancoPI.Enabled = IIf(dt.Rows(0)("sn_num_banco") = -1, True, False)
        txtDomBancoPI.Enabled = IIf(dt.Rows(0)("sn_domicilio") = -1, True, False)
        txtCuentaPI.Enabled = IIf(dt.Rows(0)("sn_cuenta") = -1, True, False)
        txtAbaPI.Enabled = IIf(dt.Rows(0)("sn_aba_routing") = -1, True, False)
        txtSwiftPI.Enabled = IIf(dt.Rows(0)("sn_swift") = -1, True, False)
        txtTransitPI.Enabled = IIf(dt.Rows(0)("sn_transit") = -1, True, False)
        txtIbanPI.Enabled = IIf(dt.Rows(0)("sn_iban") = -1, True, False)

    End Sub
    Private Sub limpiarCtrlesPagoInter()
        txtBancoPI.Text = ""
        txtNroBancoPI.Text = ""
        txtDomBancoPI.Text = ""
        txtCuentaPI.Text = ""
        txtAbaPI.Text = ""
        txtSwiftPI.Text = ""
        txtTransitPI.Text = ""
        txtIbanPI.Text = ""
    End Sub
    Private Sub limpiarCtrlesTriangulado()
        txtTrianNomBancoPI.Text = ""
        txtTrianCuentaPI.Text = ""
        txtTrianAbaPI.Text = ""
    End Sub


    Private Function ValidaDatosPagoInter() As Boolean
        Dim datosPagoInter As String
        Dim concepto2 As String
        Dim dt As New DataTable

        'dt.Columns.Add("nro_OP")
        datosPagoInter = drPaisPI.SelectedValue.ToString() + "|" +
            txtBancoPI.Text.Trim() + "|" +
            txtNroBancoPI.Text.Trim() + "|" +
            txtDomBancoPI.Text.Trim() + "|" +
            txtCuentaPI.Text.Trim() + "|" +
            txtAbaPI.Text.Trim() + "|" +
            txtSwiftPI.Text.Trim() + "|" +
            txtTransitPI.Text.Trim() + "|" +
            txtIbanPI.Text.Trim() + "|" +
            IIf(chkTrianguladoPI.Checked = True, "-1", "0") + "|" +
            txtTrianNomBancoPI.Text.Trim() + "|" +
            txtTrianCuentaPI.Text.Trim() + "|" +
            txtTrianAbaPI.Text.Trim()

        dt.Columns.Add("cod_pais")
        dt.Columns.Add("banco")
        dt.Columns.Add("num_banco")
        dt.Columns.Add("domicilio")
        dt.Columns.Add("cuenta")
        dt.Columns.Add("aba_routing")
        dt.Columns.Add("swift")
        dt.Columns.Add("transit")
        dt.Columns.Add("iban")
        dt.Columns.Add("triangulado")
        dt.Columns.Add("banco_triang")
        dt.Columns.Add("cuenta_triang")
        dt.Columns.Add("aba_routing_triang")
        Try
            concepto2 = ""

            'concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblPaisPI.Text.Trim() + ": " + drPaisPI.SelectedItem.ToString()


            If drPaisPI.SelectedValue <> -1 Then

                If txtBancoPI.Enabled = True Then
                    If txtBancoPI.Text <> "" Then
                        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblBancoPI.Text.Trim() + ": " + txtBancoPI.Text.Trim()
                    Else
                        Return False
                    End If
                End If

                If txtNroBancoPI.Enabled = True Then
                    If txtNroBancoPI.Text <> "" Then
                        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblNroBancoPI.Text.Trim() + ": " + txtNroBancoPI.Text.Trim()
                    Else
                        Return False
                    End If
                End If

                If txtDomBancoPI.Enabled = True Then
                    If txtDomBancoPI.Text <> "" Then
                        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblDomBancoPI.Text.Trim() + ": " + txtDomBancoPI.Text.Trim() + ", " + drPaisPI.SelectedItem.ToString()
                        'concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblPaisPI.Text.Trim() + ": " + drPaisPI.SelectedItem.ToString()
                    Else
                        Return False
                    End If
                End If

                If txtCuentaPI.Enabled = True Then
                    If txtCuentaPI.Text <> "" Then
                        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblCuentaPI.Text.Trim() + ": " + txtCuentaPI.Text.Trim()
                    Else
                        Return False
                    End If
                End If

                If txtAbaPI.Enabled = True Then
                    If txtAbaPI.Text <> "" Then
                        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblAbaPI.Text.Trim() + ": " + txtAbaPI.Text.Trim()
                    Else
                        Return False
                    End If
                End If

                If txtSwiftPI.Enabled = True Then
                    If txtSwiftPI.Text <> "" Then
                        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblSwiftPI.Text.Trim() + ": " + txtSwiftPI.Text.Trim()
                    Else
                        Return False
                    End If
                End If
                'kjkj
                If txtTransitPI.Enabled = True Then
                    If txtTransitPI.Text <> "" Then
                        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblTransitPI.Text.Trim() + ": " + txtTransitPI.Text.Trim()
                    Else
                        Return False
                    End If
                End If

                If txtIbanPI.Enabled = True Then
                    If txtIbanPI.Text <> "" Then
                        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblIbanPI.Text.Trim() + ": " + txtIbanPI.Text.Trim()
                    Else
                        Return False
                    End If
                End If


                'If txtIbanPI.Enabled = True Then
                '    If txtIbanPI.Text <> "" Then
                '        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblTrianguladoPI.Text.Trim() + ": " + chkTrianguladoPI.Text.Trim()
                '    Else
                '        Return False
                'End If
                'End If

                'If chkTrianguladoPI.Checked Then
                '    'concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblTrianguladoPI.Text.Trim() + ": " + chkTrianguladoPI.Text.Trim()
                '    concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + "Pago Triangulado, se anexa información de banco: "

                '    If txtTrianNomBancoPI.Text <> "" Then
                '        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblTrianNomBancoPI.Text.Trim() + ": " + txtTrianNomBancoPI.Text.Trim()
                '    Else
                '        Return False
                '    End If

                '    If txtTrianCuentaPI.Text <> "" Then
                '        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblTrianCuentaPI.Text.Trim() + ": " + txtTrianCuentaPI.Text.Trim()
                '    Else
                '        Return False
                '    End If
                '    If txtTrianAbaPI.Text <> "" Then
                '        concepto2 = concepto2 + IIf(concepto2 = "", "", "-") + lblTrianAbaPI.Text.Trim() + ": " + txtTrianAbaPI.Text.Trim()
                '    Else
                '        Return False
                '    End If
                'End If


                If chkTrianguladoPI.Checked Then

                    If txtTrianNomBancoPI.Text.Trim() = "" Then
                        Return False
                    End If

                    If txtTrianCuentaPI.Text.Trim() = "" Then
                        Return False
                    End If

                    If txtTrianAbaPI.Text.Trim() = "" Then
                        Return False
                    End If
                End If

                Dim row As DataRow = dt.NewRow()

                row("cod_pais") = drPaisPI.SelectedValue
                row("banco") = txtBancoPI.Text.Trim()
                row("num_banco") = txtNroBancoPI.Text.Trim()
                row("domicilio") = txtDomBancoPI.Text.Trim()
                row("cuenta") = txtCuentaPI.Text.Trim()
                row("aba_routing") = txtAbaPI.Text.Trim()
                row("swift") = txtSwiftPI.Text.Trim()
                row("transit") = txtTransitPI.Text.Trim()
                row("iban") = txtIbanPI.Text.Trim()
                row("triangulado") = IIf(chkTrianguladoPI.Checked = True, -1, 0)
                row("banco_triang") = txtTrianNomBancoPI.Text.Trim()
                row("cuenta_triang") = txtTrianCuentaPI.Text.Trim()
                row("aba_routing_triang") = txtTrianAbaPI.Text.Trim()
                dt.Rows.Add(row)

            Else
                Return False
            End If

            HiddenFieldPI.Value = concepto2
            hidDatosPagoInter.Value = datosPagoInter

            Session("dtPI") = ""
            Session("dtPI") = dt



            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function


End Class


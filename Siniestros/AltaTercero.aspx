<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="AltaTercero.aspx.vb" Inherits="Siniestros_AltaTercero" %>

<%@ MasterType VirtualPath="~/Pages/SiteMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" runat="Server">
    <script src="../Scripts/Siniestros/AltaTercero.js"></script>


    <div class="zona-principal" style="overflow-x: hidden; overflow-y: hidden">

        <div class="cuadro-titulo panel-encabezado" style="text-align: left; tab-size: 18px">
            <%--<input type="image" src="../Images/contraer_mini_inv.png" id="coVentana0" class="contraer" />
            <input type="image" src="../Images/expander_mini_inv.png" id="exVentana0" class="expandir"/>--%>
            &nbsp&nbsp Terceros
        </div>

        <div class="panel-contenido ventana0">
            <asp:UpdatePanel runat="server" ID="upFiltros" >
                <ContentTemplate>
                    <asp:HiddenField runat="server" ID="hid_Ventanas" Value="0|1" />
                    <asp:HiddenField runat="server" ID="hid_Alta_Tercero" Value="0" />
                    <div class="padding10"></div>


                    <div class="row">
                        <div class="col-md-6">
                            <asp:HiddenField runat="server" ID="hid_codTercero" Value="0" />
                            <asp:Label runat="server" class="col-md-2 etiqueta-control" Width="20%" Height="20px" ID="lblCodTercero">Código Tercero</asp:Label>
                            <asp:TextBox runat="server" ID="txt_codTercero" CssClass="col-md-1 estandar-control" PlaceHolder="Ingresar Código/Nombre " Width="25%" Height="22px" ></asp:TextBox>



                            <asp:LinkButton ID="btnBuscar" runat="server" class="btn btn-primary btn-xs" Style="background-color: #003A5D; height: 22px; margin-right: 5px; margin-left: 5px">
                            <span>
                                <img class="btn-buscar"/> &nbsp&nbsp Buscar
                            </span>
                            </asp:LinkButton>

                            <asp:LinkButton ID="btnNuevo" runat="server" class="btn btn-primary btn-xs" Style="background-color: #003A5D; height: 22px">
                                <span>
                                    <img class="btn-añadir"/>&nbsp&nbsp Nuevo
                                </span>
                            </asp:LinkButton>
                        </div>
                        <div class="col-md-6"></div>
                        <div class="col-md-6" style="text-align: right">
                        </div>
                    </div>

                    <div class="padding30"></div>

                    <div class="row">
                        <div class="col-md-12">
                            <asp:Label runat="server" class="etiqueta-control" Width="100px">Tipo Persona</asp:Label>
                            <asp:CheckBox runat="server" ID="chkFisica" Text="&nbsp&nbspFísica" CssClass="etiqueta-control" Width="70px"  Enabled="False" />
                            <asp:CheckBox runat="server" ID="chkMoral" Text="&nbsp&nbspMoral" CssClass="etiqueta-control" Enabled="False" />
                        </div>
                    </div>

                    <div class="padding20"></div>

                    <div class="row">
                        <div class="col-md-3">
                            <asp:Label runat="server" class="etiqueta-control" Width="100%">Apellido Paterno / Razón Social</asp:Label>
                            <asp:TextBox runat="server" ID="txt_apPat" CssClass="estandar-control Tablero Centro" OnFocusOut="convMayusculas('txt_apPat')" Width="100%" Enabled="False"></asp:TextBox>

                        </div>
                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control" Width="100%">Apellido Materno</asp:Label>
                            <asp:TextBox runat="server" ID="txt_apMat" CssClass="estandar-control Tablero Centro" OnFocusOut="convMayusculas('txt_apMat')" Width="90%" Enabled="False"></asp:TextBox>

                        </div>
                        <div class="col-md-3">

                            <asp:Label runat="server" class="etiqueta-control" Width="100%">Nombre(s)</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nombres" CssClass="estandar-control Tablero Centro" OnFocusOut="convMayusculas('txt_nombres')" Width="100%" Enabled="False"></asp:TextBox>
                        </div>
                        <div class="col-md-2">

                            <asp:Label runat="server" class="etiqueta-control " Width="100%">RFC.</asp:Label>
                            <asp:TextBox runat="server" ID="txt_rfc" CssClass="estandar-control Tablero Centro" Width="100%" onkeypress="return charRFC(event)"  Enabled="False" OnFocusOut="convMayusculas('txt_rfc')"></asp:TextBox>
                            <%--<asp:TextBox runat="server" ID="txt_cod_postal" CssClass="col-md-3 estandar-control" Width="20%" PlaceHolder="Ej: 14040" onkeypress="return soloNumeros(event)"></asp:TextBox>
                            <asp:LinkButton ID="btnBuscar" runat="server" class="col-md-3 estandar-control btn botones" BorderWidth="2" BorderColor="White" Width="8%">
                                <span style="vertical-align: middle">
                                    <img class="btn-buscar" style="vertical-align: top"/>
                                </span>
                            </asp:LinkButton>--%>


                            <%--<asp:Label runat="server" class="col-md-3 etiqueta-control" Width="15%">&nbsp&nbsp Teléfono</asp:Label>
                            <asp:TextBox runat="server" ID="txt_telefono" CssClass="col-md-3 estandar-control" Width="35%" PlaceHolder="Ej: 51331000 EXT. 1745" OnFocusOut="convMayusculas('txt_telefono')" Enabled="True"></asp:TextBox>--%>
                        </div>
                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Sexo</asp:Label>
                            <asp:DropDownList runat="server" ID="drSexo" CssClass="estandar-control Tablero" Width="100%" Enabled="False" >
                                <asp:ListItem Text="FEMENINO" Value="F" />
                                <asp:ListItem Text="MASCULINO" Value="M" Selected="true" />
                            </asp:DropDownList>
                        </div>

                    </div>
                    <div class="padding20"></div>


                    <div class="row">
                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Estado Civil</asp:Label>
                            <asp:DropDownList runat="server" ID="drEdoCivil" CssClass="estandar-control Tablero Centro" Width="100%" Enabled="False" ></asp:DropDownList>
                        </div>

                        <div class="col-md-2">

                            <asp:Label runat="server" class="etiqueta-control" Width="100%">Fecha Nacimiento</asp:Label>
                            <asp:TextBox runat="server" ID="txt_fecNac" CssClass="estandar-control Tablero Fecha Centro" Width="100%" PlaceHolder="dd/mm/aaaa"  Enabled="False" OnFocusOut="obtenerEdad()"></asp:TextBox>

                        </div>

                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Lugar Nacimiento</asp:Label>
                            <asp:TextBox runat="server" ID="txt_lugNac" CssClass="estandar-control Tablero Centro" Width="100%"  Enabled="False" OnFocusOut="convMayusculas('txt_lugNac')"></asp:TextBox>
                        </div>

                        <div class="col-md-1">

                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Edad</asp:Label>
                            <asp:TextBox runat="server" ID="txt_edad" CssClass="estandar-control Tablero Centro" Width="100%" ></asp:TextBox>
                        </div>

                        <div class="col-md-3">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Ocupación</asp:Label>
                            <asp:TextBox runat="server" ID="txt_ocup" CssClass="estandar-control Tablero Centro" Width="100%"  Enabled="False" OnFocusOut="convMayusculas('txt_ocup')"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Sueldo Mensual</asp:Label>
                            <asp:TextBox runat="server" ID="txt_sueldo" CssClass="estandar-control Tablero Centro" Width="100%"   Enabled="False" onkeypress="return importe(event)" OnFocusOut="formatoMoneda('txt_sueldo')" ></asp:TextBox>
                        </div>
                    </div>

                    <div class="padding20"></div>


                    <div class="row">
                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Teléfono Casa</asp:Label>
                            <asp:TextBox runat="server" ID="txt_telCasa" CssClass="estandar-control Tablero Centro" Width="100%"  Enabled="False"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Teléfono Trabajo</asp:Label>
                            <asp:TextBox runat="server" ID="txt_telTrab" CssClass="estandar-control Tablero Centro" Width="100%"  Enabled="False"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Celular</asp:Label>
                            <asp:TextBox runat="server" ID="txt_cel" CssClass="estandar-control Tablero Centro" Width="100%"  Enabled="False"></asp:TextBox>
                        </div>

                        <%--<div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%"> Fax</asp:Label>
                            <asp:TextBox runat="server" ID="txt_fax" CssClass="estandar-control Tablero Centro" Width="100%" onkeypress="return telefono(event, 'txt_fax')" AutoPostBack="true" Enabled="True"></asp:TextBox>
                        </div>--%>


                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Parentesco</asp:Label>
                            <asp:TextBox runat="server" ID="txt_paren" CssClass="estandar-control Tablero Centro" Width="100%"  Enabled="False" OnFocusOut="convMayusculas('txt_paren')"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Nombre Pariente</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nomPariente" CssClass="estandar-control Tablero Centro" Width="100%"  Enabled="False" OnFocusOut="convMayusculas('txt_nomPariente')"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">RFC Pariente</asp:Label>
                            <asp:TextBox runat="server" ID="txt_parRFC" CssClass="estandar-control Tablero Centro" Width="100%"  Enabled="False" OnFocusOut="convMayusculas('txt_parRFC')"></asp:TextBox>
                        </div>
                    </div>

                    <div class="padding20"></div>


                    <div class="row">
                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Correo electrónico</asp:Label>
                            <asp:TextBox runat="server" ID="txt_mail" CssClass="estandar-control Tablero Centro" Width="100%"  Enabled="False"></asp:TextBox>
                        </div>
                        <div class="col-md-1">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Tipo Dirección</asp:Label>
                            <asp:DropDownList runat="server" ID="drTipoDir" CssClass="estandar-control Tablero" Width="100%" Enabled="False" >
                                <asp:ListItem Text="FÍSICA" Value="1" Selected="true" />
                                <asp:ListItem Text="POSTAL" Value="2" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control" Width="100%">País</asp:Label>
                            <asp:DropDownList runat="server" ID="drPais" CssClass="estandar-control Tablero Centro" Width="100%" AutoPostBack="True" Enabled="False"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control" Width="100%">Estado</asp:Label>
                            <asp:DropDownList runat="server" ID="drEstado" CssClass="estandar-control Tablero Centro" Width="100%" AutoPostBack="True" Enabled="False"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control" Width="100%">Delegación o Municipio</asp:Label>
                            <asp:DropDownList runat="server" ID="drDeleg" CssClass="estandar-control Tablero Centro" Width="100%" AutoPostBack="True" Enabled="False"></asp:DropDownList>
                        </div>
                        <div class="col-md-1">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Código Postal</asp:Label>
                            <asp:TextBox runat="server" ID="txt_cod_postal" CssClass="estandar-control Tablero Centro" Width="100%" PlaceHolder="Ej: 14040" onkeypress="return soloNumeros(event)"  Enabled="False"></asp:TextBox>
                        </div>
                         <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control " Width="100%">Avenida</asp:Label>
                            <asp:DropDownList runat="server" ID="drCalle" CssClass="estandar-control Tablero Centro" Width="100%" Enabled="False" ></asp:DropDownList>
                        </div>
                    </div>




                    <div class="padding20"></div>

                    <div class="row">
                        <div class="col-md-3">
                            <asp:Label runat="server" class="etiqueta-control" Width="100%">Urb./Barrio/Sector</asp:Label>
                            <asp:TextBox runat="server" ID="txtNombreRural" CssClass="estandar-control Tablero Centro" OnFocusOut="convMayusculas('txtNombreRural')" Width="100%" Enabled="False"></asp:TextBox>
                        </div>

                        <div class="col-md-1">
                            <asp:Label runat="server" class="etiqueta-control" Width="100%">Nro. Casa</asp:Label>
                            <asp:TextBox runat="server" ID="txtNroCasa" CssClass="estandar-control Tablero Centro" onkeypress="return soloNumeros(event)" Width="100%" Enabled="False"></asp:TextBox>
                        </div>

                       

                        <div class="col-md-3">
                            <asp:Label runat="server" class="etiqueta-control" Width="100%">Nombre Calle</asp:Label>
                            <asp:TextBox runat="server" ID="txtNomCalle" CssClass="estandar-control Tablero Centro" OnFocusOut="convMayusculas('txtNomCalle')" Width="100%" Enabled="False"></asp:TextBox>
                        </div>

                        

                        <div class="col-md-2">
                            <asp:Label runat="server" class="etiqueta-control" Width="100%">Aptmnt/Ofic.</asp:Label>
                            <asp:TextBox runat="server" ID="txtNroApto" CssClass="estandar-control Tablero Centro" OnFocusOut="convMayusculas('txtNroApto')" Width="100%" Enabled="False"></asp:TextBox>
                        </div>
                    </div>

                    <div class="padding20"></div>

                    <div class="row">
                    </div>

                    <div class="padding30"></div>

                    <div style="width: 100%; text-align: right;">
                        <div class="padding30">
                            <asp:UpdatePanel runat="server" ID="upGuardar">
                                <ContentTemplate>
                                    <asp:LinkButton ID="btnRegresar" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="100px" Visible="False">
                                        <span>
                                            <img class="btn-refresh"/>&nbsp&nbsp Regresar
                                        </span>
                                    </asp:LinkButton>
                                    <%--VZAVALETA_10290_CC_INICIO--%>
                                    <asp:LinkButton ID="btnLimpiar" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="130px">
                                        <span>
                                            <img class="btn-limpiar"/>&nbsp&nbsp Limpiar
                                        </span>
                                    </asp:LinkButton>
                                    <%--VZAVALETA_10290_CC_FIN--%>
                                    <asp:LinkButton ID="btnEditar" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="100px" Visible="False">
                                        <span>
                                            <img class="btn-modificar"/>&nbsp&nbsp Editar
                                        </span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnGuardar" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="100px" Visible="False">
                                        <span>
                                            <img class="btn-guardar"/>&nbsp&nbsp Guardar
                                        </span>
                                    </asp:LinkButton>

                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>


                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div id="ModConfirmar" class="modal-catalogo">
        <div class="cuadro-titulo-flotante">
            <div class="padding5"></div>
            <button type="button" data-dismiss="modal" class="close" hidden="hidden">&times;</button>
            <div>
                <label id="lbl_conf">Guardar</label>
            </div>
            <div class="padding5"></div>
        </div>
        <div class="padding5"></div>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <div class="input-group">
                    <br />
                    <div class="padding10"></div>
                    <asp:Label runat="server" class="col-md-12 etiqueta-control" Font-Size="13px">Desea guardar los cambios realizados?
                    </asp:Label>
                    <div class="padding30"></div>
                    <br />
                    <br />
                </div>
                <div style="width: 100%; text-align: right;">
                    <asp:Button runat="server" ID="btnSi" class="btn botones" Text="SI" />
                    <asp:Button runat="server" ID="btnNo" class="btn botones" data-dismiss="modal" Text="NO" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>


    <div id="Procesado" class="modal-catalogo">
        <div class="cuadro-titulo-flotante">
            <div class="padding5"></div>
            <button type="button" data-dismiss="modal" class="close" hidden="hidden">&times;</button>
            <div>
                <label id="lbl_proc">Guardado</label>
            </div>
            <div class="padding5"></div>
        </div>
        <div class="padding5"></div>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <div class="input-group">
                    <br />
                    <div class="padding10"></div>
                    <asp:Label runat="server" class="col-md-12 etiqueta-control" Font-Size="13px">Datos guardados correctamente. 
                    </asp:Label>
                    <asp:Label runat="server" class="col-md-12 etiqueta-control" ID="lblClaveNueva" Text="" />
                    <div class="padding30"></div>
                    <br />
                    <br />
                </div>
                <div style="width: 100%; text-align: right;">
                    <asp:Button runat="server" ID="btnAcepProc" class="btn botones" Text="Aceptar" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

</asp:Content>



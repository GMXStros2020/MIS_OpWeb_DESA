<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="DatosEnvioChequeABM.aspx.vb" Inherits="Siniestros_DatosEnvioChequeABM" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" runat="Server">
    <script src="../Scripts/Siniestros/DatosEnvioCheque.js"></script>


    <div class="zona-principal" style="overflow-x: hidden; overflow-y: hidden">

        <div class="cuadro-titulo panel-encabezado" style="text-align: left; tab-size: 18px">
            <%--<input type="image" src="../Images/contraer_mini_inv.png" id="coVentana0" class="contraer" />
            <input type="image" src="../Images/expander_mini_inv.png" id="exVentana0" class="expandir"/>--%>
            &nbsp&nbsp Datos Envío Cheque
        </div>

        <div class="panel-contenido ventana0">
            <asp:UpdatePanel runat="server" ID="upFiltros">
                <ContentTemplate>
                    <asp:HiddenField runat="server" ID="hid_Ventanas" Value="0|1" />
                    <div class="padding10"></div>


                    <div class="row">
                        <div class="col-md-6">
                            <asp:HiddenField runat="server" ID="hid_clave" Value="0" />
                            <asp:Label runat="server" class="col-md-2 etiqueta-control" Width="25%" Height="30px" ID="lblClave">Clave</asp:Label>
                            <asp:TextBox runat="server" ID="txt_clave" CssClass="col-md-1 estandar-control" PlaceHolder="Ej: 115" Width="30%" Height="30px" onkeypress="return soloNumeros(event)" ></asp:TextBox>
                            <%--<asp:TextBox runat="server" ID="txt_clave" CssClass="col-md-1 estandar-control" PlaceHolder="Ej: 115" Width="30%" Height="30px" onkeypress="return soloNumeros(event)" OnTextChanged="txt_clave_TextChanged" AutoPostBack="true" ></asp:TextBox>--%>
                                                        
                            <asp:LinkButton ID="btnBuscar" runat="server" class="btn botones" Width="90px" Height="30px" style="margin-right: 5px; margin-left:5px">
                                <span>
                                    <img class="btn-buscar"/>&nbsp&nbsp Buscar
                                </span>
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="btnNuevo" runat="server" class="btn botones" Width="90px" Height="30px" >
                                <span>
                                    <img class="btn-añadir"/>&nbsp&nbsp Nuevo
                                </span>
                            </asp:LinkButton>
                            



                        </div>
                        <div class="col-md-6"></div>
                        <div class="col-md-6" style="text-align: right">
                                 <asp:LinkButton ID="btnExportar" runat="server" class="btn botones" Width="150px" Height="30px" >
                                <span>
                                    <img class="btn-excel"/>&nbsp&nbsp Exportar Catálogo
                                </span>
                            </asp:LinkButton>
                        </div>
                    </div>

                    <div class="padding20"></div>

                    <div class="row">
                        <div class="col-md-6"></div>

                        <div class="col-md-6">
                            <asp:Label runat="server" class="col-md-1 etiqueta-control" Width="0%"></asp:Label>
                            <asp:CheckBox runat="server" ID="chkActivo" CssClass="etiqueta-control" Text="&nbsp&nbspActivo" Enabled="False" />
                        </div>
                    </div>

                    <div class="padding20"></div>

                    <div class="row">
                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-2 etiqueta-control" Width="25%" >Nombre Completo</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nombre" CssClass="col-md-1 estandar-control" PlaceHolder="Ej: C.P. ARTURO DOMÍNGUEZ GONZÁLEZ" OnFocusOut="convMayusculas('txt_nombre')" Width="75%" Enabled="False"></asp:TextBox>
                        </div>

                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-1 etiqueta-control " Width="15%" >Empresa</asp:Label>
                            <asp:TextBox runat="server" ID="txt_empresa" CssClass="col-md-1 estandar-control" Width="85%" OnFocusOut="convMayusculas('txt_empresa')" placeHolder="Ej: GRUPO MEXICANO DE SEGUROS, S.A. de C.V." Enabled="False"></asp:TextBox>
                        </div>
                    </div>
                    <div class="padding10"></div>

                    <div class="row">
                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-2 etiqueta-control" Width="25%">Estado</asp:Label>
                            <asp:DropDownList runat="server" ID="drEstado" CssClass="col-md-3 estandar-control" Width="75%" AutoPostBack="True" Enabled="False"></asp:DropDownList>
                        </div>

                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-1 etiqueta-control " Width="15%">Ciudad</asp:Label>
                            <asp:DropDownList runat="server" ID="drCiudad" CssClass="col-md-3 estandar-control" Width="85%" Enabled="false" AutoPostBack="True"></asp:DropDownList>
                        </div>
                    </div>


                    <div class="padding10"></div>
                    <div class="row">
                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-2 etiqueta-control" Width="25%">Deleg. o Mun.</asp:Label>
                            <asp:DropDownList runat="server" ID="drDeleg" CssClass="col-md-3 estandar-control" Width="75%" Enabled="false" AutoPostBack="True"></asp:DropDownList>
                        </div>

                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-1 etiqueta-control " Width="15%">Colonia</asp:Label>
                            <asp:DropDownList runat="server" ID="drColonia" CssClass="col-md-3 estandar-control" Width="85%" Enabled="false" AutoPostBack="True"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="padding10"></div>

                    <div class="row">
                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-2 etiqueta-control" Width="25%">Calle</asp:Label>
                            <asp:TextBox runat="server" ID="txt_calle" CssClass="col-md-1 estandar-control" Width="75%" placeHolder="Ej:  PERIFERICO SUR NO. 5450" OnFocusOut="convMayusculas('txt_calle')" Enabled="False"></asp:TextBox>
                        </div>

                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-3 etiqueta-control " Width="15%">C.P.</asp:Label>
                            <asp:TextBox runat="server" ID="txt_cod_postal" CssClass="col-md-3 estandar-control" Width="35%" PlaceHolder="Ej: 14040" onkeypress="return soloNumeros(event)" AutoPostBack="true" Enabled="False"></asp:TextBox>
                            <%--<asp:TextBox runat="server" ID="txt_cod_postal" CssClass="col-md-3 estandar-control" Width="20%" PlaceHolder="Ej: 14040" onkeypress="return soloNumeros(event)"></asp:TextBox>
                            <asp:LinkButton ID="btnBuscar" runat="server" class="col-md-3 estandar-control btn botones" BorderWidth="2" BorderColor="White" Width="8%">
                                <span style="vertical-align: middle">
                                    <img class="btn-buscar" style="vertical-align: top"/>
                                </span>
                            </asp:LinkButton>--%>


                            <asp:Label runat="server" class="col-md-3 etiqueta-control" Width="15%">&nbsp&nbsp Teléfono</asp:Label>
                            <asp:TextBox runat="server" ID="txt_telefono" CssClass="col-md-3 estandar-control" Width="35%" PlaceHolder="Ej: 51331000 EXT. 1745" OnFocusOut="convMayusculas('txt_telefono')" Enabled="False"></asp:TextBox>
                        </div>
                    </div>

                    <div class="padding10"></div>

                    <div style="width: 100%; text-align: right;">
                        <div class="padding30">
                            <asp:UpdatePanel runat="server" ID="upGuardar">
                                <ContentTemplate>
                                    <asp:LinkButton ID="btnRegresar" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="145px" Visible="False">
                                        <span>
                                            <img class="btn-refresh"/>&nbsp&nbsp Regresar
                                        </span>
                                    </asp:LinkButton>
                                     <asp:LinkButton ID="btnEditar" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="145px" Visible="False">
                                        <span>
                                            <img class="btn-modificar"/>&nbsp&nbsp Editar
                                        </span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnGuardar" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="145px" Visible="False">
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

     <div id="ModConfirmar" class="modal-catalogo" >
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
                    <asp:Label runat="server" class="col-md-12 etiqueta-control" ID="lblClaveNueva" Text=""/>
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
    
<%--VZAVALETA_10290_Incidencias_ini--%>
    <div class="row">
        <center>
            <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional" >
                <ContentTemplate>   
                    <asp:Panel runat="server" ID="pnlUsuario" Width="100%" Height="300" ScrollBars="Auto" >
                        <asp:GridView ID="grd" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#003A5D" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" Width="95%">
                        <AlternatingRowStyle BackColor="#DCDCDC" />

                            <Columns>                                                 
                                <asp:BoundField DataField="clave" HeaderText="Clave" />
                                <asp:BoundField DataField="txt_empresa" HeaderText="Empresa" />
                                <asp:BoundField DataField="txt_atencion" HeaderText="Atención" />
                                <asp:BoundField DataField="txt_dpto" HeaderText="Cod dpto." />  
                                <asp:BoundField DataField="txt_ciudad" HeaderText="Ciudad" />
                                <asp:BoundField DataField="txt_municipio" HeaderText="Municipio" />
                                <asp:BoundField DataField="txt_colonia" HeaderText="Colonia" />
                                <asp:BoundField DataField="txt_calle" HeaderText="Calle" />
                                <asp:BoundField DataField="txt_cod_postal" HeaderText="Código postal" />
                                <asp:BoundField DataField="txt_telefonos" HeaderText="Teléfono" />
                                <asp:BoundField DataField="txt_activo" HeaderText="Activo" />                                    
                            </Columns>

                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <HeaderStyle BackColor="#003A5D" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                            <SortedAscendingCellStyle BackColor="#F1F1F1" />
                            <SortedAscendingHeaderStyle BackColor="#0000A9" />
                            <SortedDescendingCellStyle BackColor="#CAC9C9" />
                            <SortedDescendingHeaderStyle BackColor="#000065" />

                        </asp:GridView>
                    </asp:Panel>
                    </Grid>
                </ContentTemplate>               
            </asp:UpdatePanel> 
        </center>
    </div>
    <br />
    <br />
    <br />
    <br />
    <br />
<%--VZAVALETA_10290_Incidencias_fin--%>
</asp:Content>


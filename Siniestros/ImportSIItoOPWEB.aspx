<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="ImportSIItoOPWEB.aspx.vb" Inherits="ImportSIItoOPWEB" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" runat="Server">


    <div class="zona-principal" style="overflow-x: hidden; overflow-y: hidden">

        <div class="cuadro-titulo panel-encabezado" style="text-align: left; tab-size: 18px">
            <%--<input type="image" src="../Images/contraer_mini_inv.png" id="coVentana0" class="contraer" />
            <input type="image" src="../Images/expander_mini_inv.png" id="exVentana0" class="expandir"/>--%>
            &nbsp&nbsp Importación Op SII a OPWEB
        </div>

        <div class="panel-contenido ventana0">
            <asp:UpdatePanel runat="server" ID="upFiltros">
                <ContentTemplate>
                    <asp:HiddenField runat="server" ID="hid_Ventanas" Value="0|1" />
                    <div class="padding10"></div>

                 <%--   <div class="row">
                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-2 etiqueta-control" Width="25%" >Módulo </asp:Label>
                            <asp:DropDownList runat="server" ID="ddl_Modulo" CssClass="col-md-1 estandar-control" AutoPostBack="True" Width="75%">
                                 <asp:ListItem Text="Seleccione módulo" Value="0"></asp:ListItem>
                                <asp:ListItem Text="OP Tradicional" Value="1"></asp:ListItem>
                                <asp:ListItem Text="OP Fondos" Value="3"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-1 etiqueta-control " Width="15%" >Folio OnBase</asp:Label>
                            <asp:TextBox runat="server" ID="txt_FolioOnBase" CssClass="col-md-1 estandar-control" Width="85%"></asp:TextBox>
                        </div>
                    </div>--%>
                     <div class="row">
                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-2 etiqueta-control" Width="25%" >Módulo </asp:Label>
                            <asp:DropDownList runat="server" ID="ddl_Modulo" CssClass="col-md-1 estandar-control" AutoPostBack="True" Width="75%">
                                 <asp:ListItem Text="Seleccione módulo" Value="0"></asp:ListItem>
                                <asp:ListItem Text="OP Tradicional" Value="1"></asp:ListItem>
                                <asp:ListItem Text="OP Fondos" Value="3"></asp:ListItem>
                            </asp:DropDownList>

                            <asp:Label runat="server" class="col-md-1 etiqueta-control " Width="25%" >Folio OnBase</asp:Label>
                            <asp:TextBox runat="server" ID="txt_FolioOnBase" CssClass="col-md-1 estandar-control" Width="75%"></asp:TextBox>

                            <div class="clear padding5"></div>
             

                            <asp:Label runat="server" class="col-md-1 etiqueta-control" Width="25%">No. Orden Pago</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nro_op" CssClass="col-md-3 estandar-control" Width="75%"></asp:TextBox>
                
                            <div class="clear padding5"></div>
                            <asp:Label runat="server" class="col-md-1 etiqueta-control " Width="25%">Nro_Siniestro</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nro_stro" CssClass="col-md-3 estandar-control" Width="75%"></asp:TextBox>

                            <div class="clear padding5"></div>
                            <asp:Label runat="server" class="col-md-1 etiqueta-control" Width="25%">Nro. Solicitud</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nro_sol" CssClass="col-md-3 estandar-control" Width="75%"></asp:TextBox>
                            <div class="clear padding5"></div>
                            <asp:Label runat="server" class="col-md-1 etiqueta-control" Width="25%">Nro. Pago</asp:Label>
                            <asp:TextBox runat="server" ID="txt_NoPago" CssClass="col-md-3 estandar-control" Width="75%"></asp:TextBox>
                            <div class="clear padding5"></div>

                            <asp:Label runat="server" id="lblanalista" class="col-md-1 etiqueta-control " Width="25%" Visible ="false" >Analista Fondos</asp:Label>
                            <asp:DropDownList runat="server" ID="ddlAnalista" CssClass="col-md-3 estandar-control" Width="75%" Visible="false"></asp:DropDownList>

                            <div class="clear padding5"></div>
                            <asp:Label runat="server" ID="lblTraspaso" class="col-md-1 etiqueta-control" Width="25%" Visible="false">Traspaso</asp:Label>
                            <asp:Checkbox  runat="server" ID="chkTraspaso" Width="75%" Visible="false"></asp:Checkbox>
                            <div class="clear padding5"></div>

                         </div>
                    </div>
                    <div class="clear padding5"></div>

    <%--                <div class="row">
                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-2 etiqueta-control" Width="25%">No. Orden Pago</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nro_op" CssClass="col-md-3 estandar-control" Width="75%"></asp:TextBox>
                        </div>

                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-1 etiqueta-control " Width="15%">Nro_Siniestro</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nro_stro" CssClass="col-md-3 estandar-control" Width="85%"></asp:TextBox>
                        </div>
                    </div>--%>
                 
                
 


                    <div class="padding10"></div>
                  <%--  <div class="row">
                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-2 etiqueta-control" Width="25%">Nro. Solicitud</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nro_sol" CssClass="col-md-3 estandar-control" Width="75%"></asp:TextBox>
                        </div>

                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-1 etiqueta-control " Width="15%">Analista Fondos</asp:Label>
                            <asp:DropDownList runat="server" ID="ddlAnalista" CssClass="col-md-3 estandar-control" Width="85%" Enabled="false"></asp:DropDownList>
                        </div>
                    </div>--%>
                    <div class="padding10"></div>

                    <div class="padding10"></div>

                    <div style="width: 100%; text-align: right;">
                        <div class="padding30">
                            <asp:UpdatePanel runat="server" ID="upGuardar">
                                <ContentTemplate>
                                  
                                    <asp:LinkButton ID="btnGuardar" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="145px">
                                        <span>
                                            <img class="btn-guardar"/>&nbsp&nbsp Importar
                                        </span>
                                    </asp:LinkButton>
                                       <asp:LinkButton ID="btnLimpiar" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="145px">
                                        <span>
                                            <img class="btn-limpiar"/>&nbsp&nbsp Limpiar
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

 
</asp:Content>


<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="ContaElectronica.aspx.vb" Inherits="Siniestros_ContaElectronica" %>

<%@ MasterType VirtualPath="~/Pages/SiteMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" Runat="Server">
    <script src="../Scripts/FirmasElectronicas.js"></script>

     <asp:HiddenField runat="server" ID="hid_Ventanas" Value="0|1|1|1|1|1|1|1|1|" />

     <%--<div style="width:1000px; min-width:1000px; overflow-x:hidden">--%>
    <div style="overflow-x: hidden">
        <div class="cuadro-titulo panel-encabezado">
            <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana0" class="contraer"  />
            <input type="image" src="../Images/expander_mini_inv.png"   id="exVentana0" class="expandir"  />
            Reporte de Factura Contabilidad Electrónica
        </div>
        <div class="panel-contenido ventana0" >
            <asp:UpdatePanel runat="server" ID="upFiltros">
                <ContentTemplate>
                   <div class="row">
                        
                        <div class="col-md-3">
                            <asp:label runat="server" class="col-md-1 etiqueta-control" Width="100px">* Fecha Desde</asp:label>
                            <asp:TextBox runat="server" ID="txtFecGeneraDe" CssClass="estandar-control Fecha Centro" Width="110px" ></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            <asp:label runat="server" class="etiqueta-control" Width="100px">A</asp:label>
                        </div>
                        <div class="col-md-2">
                            <asp:TextBox runat="server" ID="txtFecGeneraA" CssClass="estandar-control Fecha Centro" Width="110px" ></asp:TextBox>
                        </div>
                      <%--VZAVALETA_GMX-10290_INCIDENCIAS--%>
                       <div class="col-sm-1">
                        <asp:label runat="server" class="etiqueta-control" Width="100px">Relacionado</asp:label>
                       </div>
                       <div class="col-md-2" style="text-align: center">
                            <asp:DropDownList ID="ddl_Relacionado" runat="server" ClientIDMode="Static" CssClass="estandar-control tipoUsuario Tablero">
                                 <asp:ListItem Value="1" >TODAS</asp:ListItem> 
                                <asp:ListItem Value="-1" >RELACIONADAS</asp:ListItem> 
                                <asp:ListItem Value="0">NO RELACIONADAS</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                       <%--VZAVALETA_GMX-10290_INCIDENCIAS--%>
                       <br />
                       <br />
                       <div style="width:100%; text-align:right; border-top-style:inset; border-width:1px; border-color:#003A5D">
                            <asp:LinkButton id="btn_Reporte" runat="server" class="btn botones">
                                <span>
                                    <img class="btn-buscar"/>
                                    Reporte
                                </span>
                            </asp:LinkButton>
                        </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            
             <%--   <asp:UpdatePanel runat="server" ID="upBusqueda">
                    <ContentTemplate>
               
                    </ContentTemplate>
                </asp:UpdatePanel>--%>
             <%--</div>--%>
        </div>
    </div>
</asp:Content>


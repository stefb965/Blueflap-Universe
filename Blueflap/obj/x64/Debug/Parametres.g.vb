﻿#ExternalChecksum("C:\Users\Sâm\Documents\Visual Studio 2015\Projects\Blueflap\Blueflap\Parametres.xaml", "{406ea660-64cf-4c82-b6f0-42d48172a799}", "4E065E21FA004B87ACA413B82D19D39A")
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Namespace Global.Blueflap

    Partial Class Parametres
        Implements Global.Windows.UI.Xaml.Markup.IComponentConnector
        Implements Global.Windows.UI.Xaml.Markup.IComponentConnector2


        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "14.0.0.0")>  _
        <Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
        Public Sub Connect(ByVal connectionId As Integer, ByVal target As Global.System.Object) Implements Global.Windows.UI.Xaml.Markup.IComponentConnector.Connect
            Select Case connectionid
            Case 1
                    Dim element1 As Global.Windows.UI.Xaml.Controls.Page = CType(target, Global.Windows.UI.Xaml.Controls.Page)
#ExternalSource("..\..\..\Parametres.xaml",8)
                AddHandler DirectCast(element1, Global.Windows.UI.Xaml.Controls.Page).Loaded, AddressOf Me.Page_Loaded
#End ExternalSource
                    Exit Select
            Case 2
                    Me.ParamOpen = CType(target, Global.Windows.UI.Xaml.Media.Animation.Storyboard)
                    Exit Select
            Case 3
                    Me.ParaPage = CType(target, Global.Windows.UI.Xaml.Controls.Grid)
                    Exit Select
            Case 4
                    Me.grid = CType(target, Global.Windows.UI.Xaml.Controls.Grid)
                    Exit Select
            Case 5
                    Me.Settings_SearchEngine = CType(target, Global.Windows.UI.Xaml.Controls.ComboBox)
                    Exit Select
            Case 6
                    Me.Theme_switch = CType(target, Global.Windows.UI.Xaml.Controls.ToggleSwitch)
                    Exit Select
            Case 7
                    Me.SearchEngine_1 = CType(target, Global.Windows.UI.Xaml.Controls.TextBox)
                    Exit Select
            Case 8
                    Me.SearchEngine_2 = CType(target, Global.Windows.UI.Xaml.Controls.TextBox)
                    Exit Select
            Case 9
                    Me.Startpage_Settings = CType(target, Global.Windows.UI.Xaml.Controls.TextBox)
                    Exit Select
            Case 10
                    Me.searchengine3 = CType(target, Global.Windows.UI.Xaml.Controls.TextBlock)
                    Exit Select
            Case 11
                    Me.Color_Switch = CType(target, Global.Windows.UI.Xaml.Controls.ToggleSwitch)
                    Exit Select
            Case 12
                    Me.Color1_But = CType(target, Global.Windows.UI.Xaml.Controls.Button)
                    Exit Select
            Case 13
                    Me.Color2_But = CType(target, Global.Windows.UI.Xaml.Controls.Button)
                    Exit Select
            Case 14
                    Me.Color3_But = CType(target, Global.Windows.UI.Xaml.Controls.Button)
                    Exit Select
            Case 15
                    Me.Color4_But = CType(target, Global.Windows.UI.Xaml.Controls.Button)
                    Exit Select
            Case 16
                    Me.loader = CType(target, Global.Windows.UI.Xaml.Controls.ProgressRing)
                    Exit Select
            Case 17
                    Dim element17 As Global.Windows.UI.Xaml.Controls.Button = CType(target, Global.Windows.UI.Xaml.Controls.Button)
#ExternalSource("..\..\..\Parametres.xaml",31)
                AddHandler DirectCast(element17, Global.Windows.UI.Xaml.Controls.Button).Tapped, AddressOf Me.Button_Tapped
#End ExternalSource
                    Exit Select
                    Case Else
                        Exit Select
            End Select
                Me._contentLoaded = true
        End Sub
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "14.0.0.0")>  _
        <Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
        Public Function GetBindingConnector(connectionId As Integer, target As Object) As Global.Windows.UI.Xaml.Markup.IComponentConnector Implements Global.Windows.UI.Xaml.Markup.IComponentConnector2.GetBindingConnector
            Dim returnValue As Global.Windows.UI.Xaml.Markup.IComponentConnector = Nothing
            Return returnValue
        End Function
    End Class

End Namespace

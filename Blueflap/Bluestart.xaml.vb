﻿' Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238
Imports Windows.ApplicationModel.Core
Imports Windows.Data.Json
Imports Windows.UI.Xaml.Documents
Imports Windows.Storage
Imports Windows.Web.Http

''' <summary>
''' Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
''' </summary>
Public NotInheritable Class Bluestart
    Inherits Page
    Dim itemcount As Integer
    Dim PreventAnimationBug As Boolean
#Region "Frame.GoBack"
    Public Sub New()
        Me.InitializeComponent()
        AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf MainPage_BackRequested
    End Sub
    Private Sub MainPage_BackRequested(sender As Object, e As Windows.UI.Core.BackRequestedEventArgs)
        'On retourne à la page principale quand le bouton retour "physique" est pressé
        If Ellipsis.Visibility = Visibility.Visible Then
            Ellipsis_Close.Begin()
            e.Handled = True
        End If
    End Sub
#End Region
#Region "Page Loaded"
    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        localSettings.Values("LoadPageFromBluestart") = False
        LoadAnim.Begin() 'Animation d'ouverture

        PreventAnimationBug = True
        Await ShowFavorites()

        'Définition du thème avec couleur personnalisée
        Try
            If localSettings.Values("CustomColorEnabled") = True Then
                Ellipsis_Button.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(localSettings.Values("CustomColorD"), localSettings.Values("CustomColorA"), localSettings.Values("CustomColorB"), localSettings.Values("CustomColorC")))
            Else
                Ellipsis_Button.Background = DefaultThemeColor.Background
            End If
        Catch
        End Try

        Ellipsis.Background = Ellipsis_Button.Background

        Dim v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
        v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)
        v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White
        v.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27)

        If localSettings.Values("DarkThemeEnabled") = True Then
            PhoneNavBar.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 21, 21, 21))
            PhoneNavBar.BorderBrush = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 70, 70, 70))
            PhoneNavBar.RequestedTheme = ElementTheme.Dark
        Else
            PhoneNavBar.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
            PhoneNavBar.BorderBrush = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 147, 147, 147))
            PhoneNavBar.RequestedTheme = ElementTheme.Light
        End If

        Try
            If localSettings.Values("WallpaperType") = "Custom" Then 'Définit l'image de fond : image prédéfinie ou image en ligne
                Loader.Visibility = Visibility.Visible
                DownloadingWallpaper.Begin()
                Wallpaper.Source = New BitmapImage(New Uri(localSettings.Values("WallpaperSource"), UriKind.Absolute))
            Else
                DownloadingWallpaper.Stop()
                Loader.Visibility = Visibility.Collapsed

                Wallpaper.Source = New BitmapImage(New Uri("ms-appx:/Assets/" + localSettings.Values("WallpaperName"), UriKind.Absolute))
            End If
        Catch ex As Exception

        End Try

        grid3.Visibility = Visibility.Collapsed

        RechercheBox.Focus(Windows.UI.Xaml.FocusState.Keyboard)
    End Sub
#End Region
#Region "LastFav"
    Private Async Sub FavoriteGrid_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles FavoriteGrid.Tapped
        randomprogress.Visibility = Visibility.Visible
        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings
        itemcount = 0
        Dim Json As String
        Dim PreventMultipleSameItems As New HashSet(Of String)()

        Try
            Json = Await ReadJsonFile("Favorites")
        Catch ex As Exception
            Json = "[]"
        End Try

        For Each histElem In JsonArray.Parse(Json).Reverse

            If Not PreventMultipleSameItems.Contains(histElem.GetObject.GetNamedString("url").ToLower) Then
                itemcount = itemcount + 1
            End If
            PreventMultipleSameItems.Add(histElem.GetObject.GetNamedString("url").ToLower)

        Next

        Try
            Json = Await ReadJsonFile("History")
        Catch ex As Exception
            Json = "[]"
        End Try

        For Each histElem In JsonArray.Parse(Json).Reverse

            If Not PreventMultipleSameItems.Contains(histElem.GetObject.GetNamedString("url").ToLower) Then
                itemcount = itemcount + 1
            End If

            PreventMultipleSameItems.Add(histElem.GetObject.GetNamedString("url").ToLower)
        Next

        Dim random As New Random()
        Dim randomNumber As Integer = random.[Next](0, itemcount)
        Dim RandomCount As Integer = 0


        Dim Json2 As String
        Dim PreventMultipleSameItems2 As New HashSet(Of String)()

        Try
            Json2 = Await ReadJsonFile("Favorites")
        Catch ex As Exception
            Json2 = "[]"
        End Try

        For Each histElem In JsonArray.Parse(Json2).Reverse

            If RandomCount < randomNumber Then
                If Not PreventMultipleSameItems2.Contains(histElem.GetObject.GetNamedString("url").ToLower) Then
                    RandomCount = RandomCount + 1
                    localSettings.Values("LoadPageFromBluestart_Adress") = histElem.GetObject.GetNamedString("url")
                End If
                PreventMultipleSameItems2.Add(histElem.GetObject.GetNamedString("url").ToLower)
            End If
        Next

        Try
            Json2 = Await ReadJsonFile("History")
        Catch ex As Exception
            Json2 = "[]"
        End Try

        For Each histElem In JsonArray.Parse(Json2).Reverse

            If RandomCount < randomNumber Then
                If Not PreventMultipleSameItems2.Contains(histElem.GetObject.GetNamedString("url").ToLower) Then
                    RandomCount = RandomCount + 1
                    localSettings.Values("LoadPageFromBluestart_Adress") = histElem.GetObject.GetNamedString("url")
                End If
                PreventMultipleSameItems2.Add(histElem.GetObject.GetNamedString("url").ToLower)
            End If

        Next
        randomprogress.Visibility = Visibility.Collapsed

        localSettings.Values("LoadPageFromBluestart") = True
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub FavoriteGrid_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles FavoriteGrid.PointerEntered
        FavoriteGrid.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(240, 255, 255, 255))
    End Sub

    Private Sub FavoriteGrid_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles FavoriteGrid.PointerExited
        FavoriteGrid.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(204, 255, 255, 255))
    End Sub
#End Region
#Region "Good Looking"
    'Ce qui suit est d'ordre visuel
    '<Visuel>
    Private Sub TextBox_GotFocus(sender As Object, e As RoutedEventArgs)
        AdressBack.Visibility = Visibility.Visible
        RechercheBox.BorderThickness = New Thickness(0, 0, 0, 0)
    End Sub

    Private Sub TextBox_LostFocus(sender As Object, e As RoutedEventArgs)
        AdressBack.Visibility = Visibility.Collapsed
        RechercheBox.BorderThickness = New Thickness(0, 0, 0, 2)
    End Sub
    '</Visuel>
#End Region
#Region "SearchBox"

    Private Sub RechercheBox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles RechercheBox.KeyDown
        If (e.Key = Windows.System.VirtualKey.Enter) Then  'Permet de réagir à l'appui sur la touche entrée
            Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

            localSettings.Values("LoadPageFromBluestart") = True
            localSettings.Values("LoadPageFromBluestart_Adress") = RechercheBox.Text
            Me.Frame.Navigate(GetType(MainPage))
        End If
    End Sub
#End Region
#Region "Go to (other frame)"
    Private Sub Button_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub Fight_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Fight_Button.Tapped
        Me.Frame.Navigate(GetType(SearchFight))
    End Sub

    Private Sub Lock_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Lock_Button.Tapped
        Me.Frame.Navigate(GetType(Verrouillage))
    End Sub
    Private Sub Personnalisation_Button_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Personnalisation_Button.Tapped
        Me.Frame.Navigate(GetType(Customization))
    End Sub
#End Region
#Region "Personnalization panel"
    ' Private Async Sub Button_Tapped_1(sender As Object, e As TappedRoutedEventArgs)
    'Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings


    'Dim picker = New Windows.Storage.Pickers.FileOpenPicker()
    '        picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail
    '        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
    '        picker.FileTypeFilter.Add(".jpg")
    '        picker.FileTypeFilter.Add(".jpeg")
    '        picker.FileTypeFilter.Add(".png")
    '
    '    Dim FichierSelectionne As Windows.Storage.StorageFile = Await picker.PickSingleFileAsync()
    '    If FichierSelectionne IsNot Nothing Then
    '    Dim Chemin = Await FichierSelectionne.OpenAsync(Windows.Storage.FileAccessMode.Read)
    '    Dim bitmapImage = New Windows.UI.Xaml.Media.Imaging.BitmapImage()
    '            Await bitmapImage.SetSourceAsync(Chemin)
    '            Wallpaper.Source = bitmapImage
    '
    '            localSettings.Values("WallpaperImagePath") = FichierSelectionne.Path
    '
    '    End If
    '    End Sub


#End Region
#Region "SmartSuggest"
    Private Sub AdressBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles RechercheBox.TextChanged
        Try
            itemcount = 0
            SmartSuggest_Histo()
        Catch
        End Try



        If RechercheBox.Text = "" Then
            SmartSuggest_History.Visibility = Visibility.Collapsed
            FavSuggest.Visibility = Visibility.Visible
            SearchLessOpacity.Stop()
            PreventAnimationBug = True
        Else
            SmartSuggest_History.Visibility = Visibility.Visible
            FavSuggest.Visibility = Visibility.Collapsed
            If PreventAnimationBug = True Then
                SearchLessOpacity.Begin()
                PreventAnimationBug = False
            End If
        End If
    End Sub
    Private Async Sub SmartSuggest_Histo()
        SmartSuggest_History.Children.Clear()
        Dim Json As String
        Dim PreventMultipleSameItems As New HashSet(Of String)()

        Try
            Json = Await ReadJsonFile("Favorites")
        Catch ex As Exception
            Json = "[]"
        End Try

        For Each histElem In JsonArray.Parse(Json).Reverse
            Dim elemContainer As StackPanel = New StackPanel
            elemContainer.Padding = New Thickness(23, 8, 0, 8)
            AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

                                                                        localSettings.Values("LoadPageFromBluestart") = True
                                                                        localSettings.Values("LoadPageFromBluestart_Adress") = histElem.GetObject.GetNamedString("url")
                                                                        Me.Frame.Navigate(GetType(MainPage))
                                                                    End Function)

            If PhoneNavBar.Visibility = Visibility.Collapsed Then
                AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                     elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(70, 52, 152, 213))
                                                                                 End Function)

                AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                    elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                                                                                End Function)

            End If
            Dim elemText As TextBlock = New TextBlock
            elemText.Text = histElem.GetObject.GetNamedString("title")
            elemText.Foreground = New SolidColorBrush(Windows.UI.Colors.White)
            elemContainer.Children.Add(elemText)

            Dim UrlText As TextBlock = New TextBlock
            UrlText.Text = histElem.GetObject.GetNamedString("url")
            elemText.Foreground = New SolidColorBrush(Windows.UI.Colors.White)
            elemContainer.Children.Add(UrlText)



            If Not PreventMultipleSameItems.Contains(histElem.GetObject.GetNamedString("url").ToLower) Then
                If histElem.GetObject.GetNamedString("title").ToLower.Contains(RechercheBox.Text.ToLower) Or histElem.GetObject.GetNamedString("url").ToLower.Contains(RechercheBox.Text.ToLower) Then
                    SmartSuggest_History.Children.Add(elemContainer)

                    itemcount = itemcount + 1
                End If
            End If

            PreventMultipleSameItems.Add(histElem.GetObject.GetNamedString("url").ToLower)

        Next

        Try
            Json = Await ReadJsonFile("History")
        Catch ex As Exception
            Json = "[]"
        End Try

        For Each histElem In JsonArray.Parse(Json).Reverse
            Dim elemContainer As StackPanel = New StackPanel
            elemContainer.Padding = New Thickness(23, 8, 0, 8)
            AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

                                                                        localSettings.Values("LoadPageFromBluestart") = True
                                                                        localSettings.Values("LoadPageFromBluestart_Adress") = histElem.GetObject.GetNamedString("url")
                                                                        Me.Frame.Navigate(GetType(MainPage))
                                                                    End Function)

            If PhoneNavBar.Visibility = Visibility.Collapsed Then
                AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                     elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(70, 52, 152, 213))
                                                                                 End Function)

                AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                    elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 52, 152, 213))
                                                                                End Function)
            End If

            Dim elemText As TextBlock = New TextBlock
            elemText.Text = histElem.GetObject.GetNamedString("title")
            elemText.Foreground = New SolidColorBrush(Windows.UI.Colors.White)
            elemContainer.Children.Add(elemText)

            Dim UrlText As TextBlock = New TextBlock
            UrlText.Text = histElem.GetObject.GetNamedString("url")
            elemText.Foreground = New SolidColorBrush(Windows.UI.Colors.White)
            elemContainer.Children.Add(UrlText)



            If Not PreventMultipleSameItems.Contains(histElem.GetObject.GetNamedString("url").ToLower) Then
                If histElem.GetObject.GetNamedString("title").ToLower.Contains(RechercheBox.Text.ToLower) Or histElem.GetObject.GetNamedString("url").ToLower.Contains(RechercheBox.Text.ToLower) Then
                    SmartSuggest_History.Children.Add(elemContainer)

                    itemcount = itemcount + 1
                End If
            End If

            PreventMultipleSameItems.Add(histElem.GetObject.GetNamedString("url").ToLower)

        Next
    End Sub
    Private Async Function ShowFavorites() As Task
        FavSuggest.Children.Clear()

        For Each favsElem In JsonArray.Parse(Await ReadJsonFile("Favorites")).Reverse

            Dim elemContainer As Grid = New Grid
            elemContainer.Margin = New Thickness(0, 7, 0, 7)
            elemContainer.VerticalAlignment = VerticalAlignment.Stretch
            elemContainer.HorizontalAlignment = HorizontalAlignment.Stretch
            elemContainer.Height = 30
            elemContainer.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0))

            Dim rond As Grid = New Grid
            rond.Height = 30
            rond.Width = 30
            rond.CornerRadius = New CornerRadius(30)
            rond.HorizontalAlignment = HorizontalAlignment.Left
            rond.VerticalAlignment = VerticalAlignment.Center
            rond.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0))
            elemContainer.Children.Add(rond)

            Dim image As Image = New Image
            image.Margin = New Thickness(5, 0, 0, 0)
            image.Width = 20
            image.Height = 20
            image.HorizontalAlignment = HorizontalAlignment.Left
            Try
                Dim str As Uri = New Uri(favsElem.GetObject.GetNamedString("url"))
                image.Source = New BitmapImage(New Uri("http://" & str.Host & "/favicon.ico", UriKind.Absolute))
            Catch ex As Exception

            End Try
            elemContainer.Children.Add(image)



            Dim elemText As TextBlock = New TextBlock
            elemText.Text = favsElem.GetObject.GetNamedString("title")
            elemText.Margin = New Thickness(35, 0, 0, 0)
            elemText.VerticalAlignment = VerticalAlignment.Center
            elemText.HorizontalAlignment = HorizontalAlignment.Left
            elemText.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
            elemContainer.Children.Add(elemText)


            FavSuggest.Children.Add(elemContainer)

            AddHandler elemContainer.Tapped, New TappedEventHandler(Function(sender As Object, e As TappedRoutedEventArgs)
                                                                        Dim localSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.LocalSettings

                                                                        localSettings.Values("LoadPageFromBluestart") = True
                                                                        localSettings.Values("LoadPageFromBluestart_Adress") = favsElem.GetObject.GetNamedString("url")
                                                                        Me.Frame.Navigate(GetType(MainPage))
                                                                    End Function)

            If PhoneNavBar.Visibility = Visibility.Collapsed Then
                AddHandler elemContainer.PointerEntered, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                     rond.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(100, 255, 255, 255))
                                                                                     elemText.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(190, 255, 255, 255))
                                                                                 End Function)

                AddHandler elemContainer.PointerExited, New PointerEventHandler(Function(sender As Object, e As PointerRoutedEventArgs)
                                                                                    rond.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0))
                                                                                    elemText.Foreground = New SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
                                                                                End Function)
            End If
        Next

    End Function
    Private Async Function ReadJsonFile(FileName As String) As Task(Of String)
        Dim localFolder As StorageFolder = ApplicationData.Current.LocalFolder
        FileName += ".json"
        Dim content As String = Nothing

        Dim textfile As StorageFile = Await localFolder.GetFileAsync(FileName)
        content = Await FileIO.ReadTextAsync(textfile)
        Return content
    End Function
    Private Sub FavSuggest_PointerEntered(sender As Object, e As PointerRoutedEventArgs) Handles FavSuggest.PointerEntered
        If PreventAnimationBug = True Then
            SearchLessOpacity.Begin()
            PreventAnimationBug = False
        End If
        Suggestions.VerticalScrollBarVisibility = ScrollBarVisibility.Auto
    End Sub

    Private Sub FavSuggest_PointerExited(sender As Object, e As PointerRoutedEventArgs) Handles FavSuggest.PointerExited
        SearchLessOpacity.Stop()
        PreventAnimationBug = True
        Suggestions.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
    End Sub

#End Region
#Region "ellipsis"
    Private Sub Customization_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Customization_Button_M.Tapped
        Me.Frame.Navigate(GetType(Customization))
    End Sub

    Private Sub Fight_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Fight_Button_M.Tapped
        Me.Frame.Navigate(GetType(SearchFight))
    End Sub

    Private Sub Lock_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Lock_Button_M.Tapped
        Me.Frame.Navigate(GetType(Verrouillage))
    End Sub

    Private Sub Settings_Button_M_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Settings_Button_M.Tapped
        Me.Frame.Navigate(GetType(Parametres))
    End Sub

    Private Sub Grid_Tapped_1(sender As Object, e As TappedRoutedEventArgs)
        If Ellipsis.Visibility = Visibility.Visible Then
            Ellipsis_Close.Begin()
        Else
            Ellipsis_Open.Begin()
        End If
    End Sub

    Private Sub DarkBackground_Ellipsis_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles DarkBackground_Ellipsis.Tapped
        Ellipsis_Close.Begin()
    End Sub

    Private Sub Phone_Forward_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Phone_Forward.Tapped
        Me.Frame.Navigate(GetType(MainPage))
    End Sub


#End Region
End Class

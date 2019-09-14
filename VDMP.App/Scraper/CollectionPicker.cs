using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace VDMP.App.Scraper
{
    public class CollectionPicker
    {
        /// <summary>Picks the folder collection.</summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<StorageFile>> PickFolderCollection()
        {
            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.VideosLibrary
            };
            folderPicker.FileTypeFilter?.Add("*");
            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(
                    "PickedFolderToken", folder);

                var files = await folder.GetFilesAsync();

                return files;
            }

            // No files were picked up or the user canceled 
            return null;
        }

        /// <summary>Picks the single item.</summary>
        /// <returns>A storage file representing the movie</returns>
        public async Task<IReadOnlyList<StorageFile>> PickSingleItem()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".mov");
            picker.FileTypeFilter.Add(".avi");
            picker.FileTypeFilter.Add(".mpg");
            picker.FileTypeFilter.Add(".mkv");

            IReadOnlyList<StorageFile> file = new[] {await picker.PickSingleFileAsync()};
            if (file != null)
                return file;
            return null;
        }
    }
}
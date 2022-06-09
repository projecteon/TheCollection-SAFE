
$AzCopy='C:\Program Files (x86)\Microsoft SDKs\Azure\AzCopy\AzCopy.exe'
$Source='/Source:C:\temp\Theedatabase\Afbeeldingen Zakjes'
$SourceThumb='/Source:"C:\temp\Theedatabase\thumbnails"'
# Dev with emulator
# $Destination='/Dest:http://127.0.0.1:10000/devstoreaccount1/images'
# $DestinationThumb='/Dest:http://127.0.0.1:10000/devstoreaccount1/thumbnails'
# $DestinationKey='Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=='

# Live
# $AccountName=''
# $Destination='/Dest:https://$AccountName.blob.core.windows.net/images/'
# $DestinationThumb='/Dest:https://$AccountName.blob.core.windows.net/thumbnails/'
# $DestinationKey=''

& $AzCopy $Source $Destination /S /BlobType:block /DestType:"Blob" /Y /DestKey:$DestinationKey /SetContentType:image/jpeg

& $AzCopy $SourceThumb $DestinationThumb /S /BlobType:block /DestType:"Blob" /Y /DestKey:$DestinationKey /SetContentType:image/jpeg
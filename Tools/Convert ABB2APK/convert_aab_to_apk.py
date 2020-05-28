import subprocess
import os.path
import sys
from zipfile import ZipFile

aab_extension=".aab"
apks_extension=".apks"

# convert argument to string
input_file = sys.argv[1] + '' 

if not input_file.endswith(aab_extension):
    print("Input file must have an {} extension!".format(aab_extension))
    exit()

# removes the 'aar' extension and replaces it with 'apk'
apks_file = ''.join([input_file[:-len(aab_extension)], apks_extension])

# runs bundletool with aab_file as the input and the apks_file as output
command = "java -jar bundletool.jar build-apks --bundle={} --output={} --mode=universal".format(input_file, apks_file).split(" ")
process = subprocess.Popen(command)
process.wait()

# converts apks_file to zip
zip_file = ''.join([apks_file[:-len(apks_extension)], ".zip"])
os.rename(apks_file, zip_file)

# unzip file
with ZipFile(zip_file, 'r') as zip:
    zip.extract('universal.apk')

# rename universal.apk to a more user-friendly name
output_file = ''.join([input_file[:-len(aab_extension)], ".apk"])
os.rename('universal.apk', output_file)

# remove zip file
os.remove(zip_file)
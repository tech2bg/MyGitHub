import datetime
import os
import subprocess
import glob
from shutil import copytree, ignore_patterns, move, copy, rmtree

# copy the whole thing over to \\wltnas4\appdata_dev_common\abbg\data\yyyymmdd
DES_DIR = os.path.join(os.path.join("\\\\wltnas4" , "appdata_dev_common"), "abbg") 

# working area to add to PATH
WORK_DIR='''C:\DevRun\InflationCurveRun'''
ARCHIVE_DIR=os.path.join(WORK_DIR, "OLD")

os.sys.path.append(WORK_DIR)
prog = os.path.join( WORK_DIR , "CSharpDevRun.exe ")
outdir = os.path.join(os.path.join(WORK_DIR, "TSR"), "InflationCurve_")

# startDate = datetime.date(2014, 1, 1)


for filename in glob.glob(outdir + '*.txt'):
    print("copying file: ", filename)
    copy(filename, ARCHIVE_DIR)
    os.remove(filename)	
	
# startDate = datetime.date(2014, 1, 1)-datetime.timedelta(1)
endDate = datetime.date.today() 
startDate = endDate - datetime.timedelta(1)

print("start date :", startDate)
print("")

def daterange (start_date, end_date):
    for n in range(int ((end_date - start_date).days)):
        yield start_date + datetime.timedelta(n) 

        		
def getDestDir(isoStr):
	strs = isoStr.split('-')
	yyyymmdd=strs[0]+strs[1]+strs[2]
	return DES_DIR+'\\'+yyyymmdd  

	 
for seq in daterange (startDate, endDate) :
    print("working on this date: " + seq.isoformat())
    # subprocess.call([PROG, seq] );
    print(prog + seq.isoformat() + "  > " + outdir + seq.isoformat() + ".txt" )
    os.system(prog + seq.isoformat() + " > " +  outdir + seq.isoformat() + ".txt")
    seqDestDir = getDestDir(seq.isoformat())
    print("" )
    ## if it does not exists, create one	
    if not os.path.exists(seqDestDir):
        os.mkdir(seqDestDir)
        os.mkdir(seqDestDir+ "\\data") 
    print("Send to TSR common area: ", outdir + seq.isoformat() + ".txt  ==> ",  seqDestDir + "\\data")
    copy(outdir + seq.isoformat() + ".txt",  seqDestDir + "\\data")
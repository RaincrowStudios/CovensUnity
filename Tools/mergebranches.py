import subprocess
import os.path

# script must be in Tools folder
dirpath = os.getcwd()
if os.path.basename(dirpath) != "Tools":
    print("This script is not located in the Tools folder!")
    exit()

# going back to parent directory
os.chdir(os.path.pardir)

# checsk if a branch exists in the current repository
def branch_exists(branch):
    git_command = 'git rev-parse --verify {}'.format(branch)
    print("Running git command:", git_command)
    output = run_git_command(git_command)
    return output is 0

# run any git command
def run_git_command(git_command):    
    git_command_split = git_command.split(' ')
    return subprocess.call(git_command_split, stdout=subprocess.DEVNULL)

# get LOCAL origin branch
origin_branch = input('Origin Branch [default is dev/2.8.6]: ') 
origin_branch = origin_branch or 'dev/2.8.6'
if not branch_exists(origin_branch):
    print(origin_branch, 'does not exist in local repository!')
    exit()
# check if REMOTE origin branch exists
remote_origin_branch="origin/{}".format(origin_branch)
if not branch_exists(remote_origin_branch):
    print(remote_origin_branch, 'does not exist in remote repository!')
    exit()

# get LOCAL target branch
target_branch = input('Target Branch [default is release/2.8.6]: ') # get target branch
target_branch = target_branch or 'release/2.8.6'
if not branch_exists(target_branch):
    print(target_branch, 'does not exist in local repository!')
    exit()
# check if REMOTE target branch exists
remote_target_branch="origin/{}".format(target_branch)
if not branch_exists(remote_target_branch):
    print(remote_target_branch, 'does not exist in remote repository!')
    exit()

if origin_branch == target_branch:
    print('Branches cannot be equal!')
    exit()

# adding all unmarked changes to index
git_command = "git add ."
run_git_command(git_command)

# saving changes to stash
git_command = "git stash save merging {} with {}".format(origin_branch, target_branch)
run_git_command(git_command)

# checkout origin branch
git_command = "git checkout {}".format(origin_branch)
output = run_git_command(git_command)
if output is not 0:
    print("Could not run", git_command)
    exit()

# set upstream to REMOTE origin branch
git_command = "git branch -u {}".format(remote_origin_branch)
output = run_git_command(git_command)
if output is not 0:
    print("Could not run", git_command)
    exit()

# pull
git_command = "git pull"
output = run_git_command(git_command)
if output is not 0:
    print("Could not run", git_command)
    exit()

# checkout target branch
git_command = "git checkout {}".format(target_branch)
output = run_git_command(git_command)
if output is not 0:
    print("Could not run", git_command)
    exit()

# set upstream to REMOTE origin branch
git_command = "git branch -u {}".format(remote_target_branch)
output = run_git_command(git_command)
if output is not 0:
    print("Could not run", git_command)
    exit()

# pull
git_command = "git pull"
output = run_git_command(git_command)
if output is not 0:
    print("Could not run", git_command)
    exit()

# merge with origin branch
git_command = "git merge {}".format(origin_branch)
output = run_git_command(git_command)
if output is not 0:
    print("Could not run", git_command)
    run_git_command("git reset --merge")
    exit()

# push changes
git_command = "git push"
output = run_git_command(git_command)
if output is not 0:
    print("Could not push changes! You will have to do them manually!")
    exit()

# go back to origin
git_command = "git checkout {}".format(origin_branch)
output = run_git_command(git_command)
if output is not 0:
    print("Could not run", git_command)
    exit()
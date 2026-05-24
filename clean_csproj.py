import os
import re

def clean_csproj(file_path):
    with open(file_path, 'r') as f:
        content = f.read()
    
    # Remove PackageReference for Microsoft.NET.Test.Sdk, NUnit, NUnit3TestAdapter
    # as they are now in Directory.Build.props
    new_content = re.sub(r'<PackageReference Include="Microsoft\.NET\.Test\.Sdk" [^>]*/>', '', content)
    new_content = re.sub(r'<PackageReference Include="Microsoft\.NET\.Test\.Sdk" [^>]*>\s*</PackageReference>', '', new_content)
    new_content = re.sub(r'<PackageReference Include="Microsoft\.NET\.Test\.Sdk" />', '', new_content)
    
    new_content = re.sub(r'<PackageReference Include="NUnit" [^>]*/>', '', new_content)
    new_content = re.sub(r'<PackageReference Include="NUnit" [^>]*>\s*</PackageReference>', '', new_content)
    
    new_content = re.sub(r'<PackageReference Include="NUnit3TestAdapter" [^>]*/>', '', new_content)
    new_content = re.sub(r'<PackageReference Include="NUnit3TestAdapter" [^>]*>\s*</PackageReference>', '', new_content)
    
    # Clean up empty ItemGroups
    new_content = re.sub(r'<ItemGroup>\s*</ItemGroup>', '', new_content)
    
    if new_content != content:
        with open(file_path, 'w') as f:
            f.write(new_content)
        return True
    return False

for root, dirs, files in os.walk('test'):
    for file in files:
        if file.endswith('.csproj'):
            file_path = os.path.join(root, file)
            if clean_csproj(file_path):
                print(f"Cleaned {file_path}")

import os
import re

def migrate_to_nunit(file_path):
    with open(file_path, 'r') as f:
        content = f.read()
    
    replacements = [
        (r'using Xunit;', 'using NUnit.Framework;'),
        (r'\[Fact\]', '[Test]'),
        (r'\[Theory\]', '[Test]'),
        (r'\[InlineData\(', '[TestCase('),
        (r'Assert\.Equal\(', 'Assert.AreEqual('),
        (r'Assert\.NotEqual\(', 'Assert.AreNotEqual('),
        (r'Assert\.True\(', 'Assert.IsTrue('),
        (r'Assert\.False\(', 'Assert.IsFalse('),
        (r'Assert\.Null\(', 'Assert.IsNull('),
        (r'Assert\.NotNull\(', 'Assert.IsNotNull('),
        (r'Assert\.Empty\(', 'Assert.IsEmpty('),
        (r'Assert\.NotEmpty\(', 'Assert.IsNotEmpty('),
        (r'Assert\.Same\(', 'Assert.AreSame('),
        (r'Assert\.NotSame\(', 'Assert.AreNotSame('),
    ]
    
    new_content = content
    for old, new in replacements:
        new_content = re.sub(old, new, new_content)
    
    if new_content != content:
        with open(file_path, 'w') as f:
            f.write(new_content)
        return True
    return False

for root, dirs, files in os.walk('test'):
    for file in files:
        if file.endswith('.cs'):
            file_path = os.path.join(root, file)
            if migrate_to_nunit(file_path):
                print(f"Migrated {file_path}")

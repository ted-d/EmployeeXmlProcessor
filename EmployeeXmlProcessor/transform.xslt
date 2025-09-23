<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

	<xsl:template match="/">
		<Employees>
			<xsl:for-each select="//item[not(@employee_id=preceding::item/@employee_id)]">
				<xsl:variable name="employeeId" select="@employee_id"/>

				<Employee id="{$employeeId}">
					<xsl:for-each select="//item[@employee_id=$employeeId]">
						<Month number="{@month}">
							<xsl:for-each select="amount">
								<Amount salary="{@salary}"/>
							</xsl:for-each>
						</Month>
					</xsl:for-each>
				</Employee>
			</xsl:for-each>
		</Employees>
	</xsl:template>
</xsl:stylesheet>